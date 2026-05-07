using Application.Features.PriceCalculationFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.PriceCalculationFeature.Commands
{
    public record CalculatePriceCommand
        (
            Guid ClientId,
            Guid? ZoneFromId,
            Guid? ZoneToId,
            string TransportMode,
            decimal? WeightKg,
            decimal? VolumeM3,
            string? ContainerType,
            int? ContainerCount,
            DateTime? Date
        ) : IRequest<ResponseHttp>
    {
        public class CalculateCommandHandler : IRequestHandler<CalculatePriceCommand, ResponseHttp>
        {
            private readonly IClientRepository _clientRepository;
            private readonly IContractRepository _contractRepository;
            private readonly ITariffGridRepository _tariffRepository;
            private readonly ISurchargeRepository _surchargeRepository;
            private readonly ITaxRuleRepository _taxRuleRepository;
            private readonly IZoneRepository _zoneRepository;
            private readonly ICurrencyRepository _currencyRepository;  // ✅ AJOUTÉ

            private readonly IMapper _mapper;

            public CalculateCommandHandler(
                IClientRepository clientRepository,
                IContractRepository contractRepository,
                ITariffGridRepository tariffRepository,
                ISurchargeRepository surchargeRepository,
                ITaxRuleRepository taxRuleRepository,
                IZoneRepository zoneRepository,
                ICurrencyRepository currencyRepository,  // ✅ AJOUTÉ
                IMapper mapper)
            {
                _clientRepository = clientRepository;
                _contractRepository = contractRepository;
                _tariffRepository = tariffRepository;
                _surchargeRepository = surchargeRepository;
                _taxRuleRepository = taxRuleRepository;
                _zoneRepository = zoneRepository;
                _currencyRepository = currencyRepository;  // ✅ AJOUTÉ
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(CalculatePriceCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // 1️⃣ Validation des entrées
                    var validationResult = await ValidateRequest(request, cancellationToken);
                    if (validationResult != null)
                        return validationResult;

                    // 2️⃣ Récupérer les données nécessaires
                    var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
                    var contract = await _contractRepository.GetActiveContractForClientAsync(request.ClientId, cancellationToken);
                    string destinationCountry = await GetDestinationCountry(request.ZoneToId, cancellationToken);

                    // Déterminer le mode de transport
                    if (!Enum.TryParse<TransportMode>(request.TransportMode, true, out var transportMode))
                    {
                        return new ResponseHttp
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Fail_Messages = $"Mode de transport invalide: {request.TransportMode}"
                        };
                    }

                    // 3️⃣ Récupérer le tariff grid avec ses lignes
                    var tariffGrid = await _tariffRepository.GetApplicableTariffGridAsync(
                        transportMode,
                        request.Date ?? DateTime.UtcNow,
                        cancellationToken);

                    if (tariffGrid == null)
                    {
                        return new ResponseHttp
                        {
                            Status = StatusCodes.Status404NotFound,
                            Fail_Messages = $"Aucune grille tarifaire applicable trouvée pour le mode {request.TransportMode}"
                        };
                    }

                    // Charger les lignes du tariff grid
                    var tariffLines = (await _tariffRepository.GetLinesByGridIdAsync(tariffGrid.Id, cancellationToken)).ToList();

                    // ✅ DÉTERMINER LA DEVISE DE LA GRILLE
                    string gridCurrencyCode = tariffGrid.CurrencyCode ?? "EUR";

                    // ✅ DÉTERMINER LA DEVISE DU CLIENT
                    string clientCurrencyCode = DetermineClientCurrency(client);

                    // ✅ DEVISE DE CALCUL (celle dans laquelle on va calculer)
                    string calculationCurrencyCode = clientCurrencyCode ?? gridCurrencyCode;

                    string currencySymbol = GetCurrencySymbol(calculationCurrencyCode);

                    // ✅ TAUX DE CONVERSION (si nécessaire)
                    decimal? conversionRate = null;
                    if (!string.IsNullOrEmpty(clientCurrencyCode) && clientCurrencyCode != gridCurrencyCode)
                    {
                        // Chercher le taux de conversion de la grille vers la devise du client
                        var exchangeRate = await _currencyRepository.ConvertAmount(
                            gridCurrencyCode,
                            clientCurrencyCode,
                            1,
                            request.Date ?? DateTime.UtcNow,
                            cancellationToken);

                        if (exchangeRate != null)
                        {
                            conversionRate = exchangeRate.Rate;
                        }
                        else
                        {
                            // Si pas de taux trouvé, on garde la devise de la grille
                            calculationCurrencyCode = gridCurrencyCode;
                            currencySymbol = GetCurrencySymbol(gridCurrencyCode);
                        }
                    }

                    // 4️⃣ Calculer le prix de base (dans la devise de la grille)
                    var (basePriceInGridCurrency, baseComponents) = await CalculateBasePrice(
                        request, tariffGrid, tariffLines, client, gridCurrencyCode, cancellationToken);

                    // ✅ APPLIQUER LA CONVERSION SI NÉCESSAIRE
                    decimal basePrice = basePriceInGridCurrency;
                    if (conversionRate.HasValue)
                    {
                        basePrice = basePriceInGridCurrency * conversionRate.Value;

                        // Ajouter un composant de conversion dans le breakdown
                        baseComponents.Add(new BaseCompoment
                        {
                            Code = "FX_CONVERSION",
                            description = $"Conversion {gridCurrencyCode} → {clientCurrencyCode} (taux: {conversionRate.Value:F4})",
                            amout = basePrice - basePriceInGridCurrency,
                            currencyCode = clientCurrencyCode,
                            calculationbasis = $"{basePriceInGridCurrency:F2} {gridCurrencyCode} × {conversionRate.Value:F4}",
                            reference = "EXCHANGE_RATE"
                        });
                    }

                    // 5️⃣ Calculer les surcharges (dans la devise calculée)
                    var (surchargesTotal, surchargeComponents) = await CalculateSurcharges(
                        request, basePriceInGridCurrency, transportMode, gridCurrencyCode, currencySymbol, cancellationToken);

                    // ✅ Convertir les surcharges si nécessaire
                    if (conversionRate.HasValue)
                    {
                        surchargesTotal = surchargesTotal * conversionRate.Value;
                        foreach (var comp in surchargeComponents)
                        {
                            comp.amout = comp.amout * conversionRate.Value;
                            comp.currencyCode = clientCurrencyCode;
                        }
                    }

                    var subtotal = basePrice + surchargesTotal;

                    // 6️⃣ Calculer les taxes (dans la devise calculée)
                    var (taxTotal, taxComponents) = await CalculateTaxes(
                        destinationCountry, subtotal, request.Date ?? DateTime.UtcNow, calculationCurrencyCode, currencySymbol, cancellationToken);

                    var totalCost = subtotal + taxTotal;

                    // 7️⃣ Construire le résultat détaillé
                    var result = new ResultDto
                    {
                        baseCost = Math.Round(basePrice, 2),
                        surchargesTotal = Math.Round(surchargesTotal, 2),
                        subtotal = Math.Round(subtotal, 2),
                        taxTotal = Math.Round(taxTotal, 2),
                        currencyCode = calculationCurrencyCode,
                        breakDown = new BreakDown
                        {
                            baseCompoments = baseComponents,
                            Surcharges = surchargeComponents,
                            Taxes = taxComponents
                        },
                        appliedContractNumber = contract?.ContractNumber ?? "N/A",
                        appliedTariffGrid = tariffGrid.Code,
                        calculatedAtDate = DateTime.UtcNow
                    };

                    return new ResponseHttp
                    {
                        Resultat = result,
                        Status = 200,
                        Fail_Messages = null
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Erreur lors du calcul du prix: {ex.Message}",
                        Status = 500
                    };
                }
            }

            // ✅ MÉTHODE POUR DÉTERMINER LA DEVISE DU CLIENT
            private string DetermineClientCurrency(Client? client)
            {
                if (client != null && !string.IsNullOrEmpty(client.DefaultCurrencyCode))
                {
                    return client.DefaultCurrencyCode.ToUpper();
                }
                return null;
            }

            // ✅ MÉTHODE POUR LE SYMBOLE DE DEVISE
            private string GetCurrencySymbol(string currencyCode)
            {
                return currencyCode switch
                {
                    "EUR" => "€",
                    "USD" => "$",
                    "GBP" => "£",
                    "TND" => "DT",
                    "JPY" => "¥",
                    "CHF" => "CHF",
                    "CAD" => "C$",
                    "AUD" => "A$",
                    _ => currencyCode
                };
            }

            private async Task<ResponseHttp?> ValidateRequest(CalculatePriceCommand request, CancellationToken cancellationToken)
            {
                var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
                if (client == null)
                {
                    return new ResponseHttp
                    {
                        Status = 400,
                        Fail_Messages = $"Client avec ID {request.ClientId} non trouvé"
                    };
                }

                if (!request.WeightKg.HasValue && !request.VolumeM3.HasValue &&
                    (!request.ContainerCount.HasValue || string.IsNullOrEmpty(request.ContainerType)))
                {
                    return new ResponseHttp
                    {
                        Status = 400,
                        Fail_Messages = "Au moins un critère de tarification doit être fourni (poids, volume ou type de conteneur)"
                    };
                }

                return null;
            }

            private async Task<string> GetDestinationCountry(Guid? zoneToId, CancellationToken cancellationToken)
            {
                if (!zoneToId.HasValue)
                    return "FR";

                var zone = await _zoneRepository.GetByIdAsync(zoneToId.Value, cancellationToken);
                return zone?.Country ?? "FR";
            }

            private async Task<(decimal total, List<BaseCompoment> components)> CalculateBasePrice(
                CalculatePriceCommand request,
                TariffGrid tariffGrid,
                List<TariffLine> tariffLines,
                Client? client,
                string currencyCode,
                CancellationToken cancellationToken)
            {
                var components = new List<BaseCompoment>();
                decimal total = 0;

                var applicableLine = SelectApplicableTariffLine(request, tariffLines);

                if (applicableLine == null)
                {
                    return new(0, new List<BaseCompoment>
                    {
                        new BaseCompoment
                        {
                            Code = "NO_TARIFF",
                            description = "Aucune ligne tarifaire applicable trouvée",
                            amout = 0,
                            currencyCode = currencyCode,
                            calculationbasis = "N/A",
                            reference = tariffGrid.Code
                        }
                    });
                }

                // Calcul au poids
                if (request.WeightKg.HasValue && request.WeightKg.Value > 0 && applicableLine.PricePerKg.HasValue)
                {
                    var ratePerKg = applicableLine.PricePerKg.Value;
                    var amount = request.WeightKg.Value * ratePerKg;
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_WT",
                        description = $"Prix base au poids ({request.WeightKg:F2} kg × {ratePerKg:F4} {currencyCode}/kg)",
                        amout = amount,
                        currencyCode = currencyCode,
                        calculationbasis = $"{request.WeightKg:F2} kg",
                        reference = tariffGrid.Code
                    });
                    total += amount;
                }

                // Calcul au volume
                if (request.VolumeM3.HasValue && request.VolumeM3.Value > 0 && total == 0 && applicableLine.PricePerM3.HasValue)
                {
                    var ratePerM3 = applicableLine.PricePerM3.Value;
                    var amount = request.VolumeM3.Value * ratePerM3;
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_VOL",
                        description = $"Prix base au volume ({request.VolumeM3:F2} m³ × {ratePerM3:F2} {currencyCode}/m³)",
                        amout = amount,
                        currencyCode = currencyCode,
                        calculationbasis = $"{request.VolumeM3:F2} m³",
                        reference = tariffGrid.Code
                    });
                    total += amount;
                }

                // Calcul par conteneur
                if (!string.IsNullOrEmpty(request.ContainerType) && request.ContainerCount.HasValue)
                {
                    var pricePerContainer = GetContainerPrice(request.ContainerType, applicableLine);
                    if (pricePerContainer > 0)
                    {
                        var amount = pricePerContainer * request.ContainerCount.Value;
                        components.Add(new BaseCompoment
                        {
                            Code = "BASE_CNT",
                            description = $"Prix base conteneurs ({request.ContainerCount} × {request.ContainerType} @ {pricePerContainer:F2} {currencyCode}/unité)",
                            amout = amount,
                            currencyCode = currencyCode,
                            calculationbasis = $"{request.ContainerCount} conteneurs",
                            reference = tariffGrid.Code
                        });
                        total += amount;
                    }
                }

                // Appliquer le prix de base si aucun calcul n'a été effectué
                if (total == 0 && applicableLine.BasePrice.HasValue && applicableLine.BasePrice.Value > 0)
                {
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_FLAT",
                        description = "Prix forfaitaire",
                        amout = applicableLine.BasePrice.Value,
                        currencyCode = currencyCode,
                        calculationbasis = "Forfait",
                        reference = tariffGrid.Code
                    });
                    total = applicableLine.BasePrice.Value;
                }

                return (total, components);
            }

            private TariffLine? SelectApplicableTariffLine(CalculatePriceCommand request, List<TariffLine> tariffLines)
            {
                var candidates = tariffLines.Where(line =>
                    (!request.ZoneFromId.HasValue || line.ZoneFromId == request.ZoneFromId || line.ZoneFromId == null) &&
                    (!request.ZoneToId.HasValue || line.ZoneToId == request.ZoneToId || line.ZoneToId == null)
                ).ToList();

                if (!candidates.Any())
                    return null;

                var withSpecificZones = candidates
                    .Where(line => line.ZoneFromId.HasValue && line.ZoneToId.HasValue)
                    .FirstOrDefault();

                if (withSpecificZones != null)
                    return withSpecificZones;

                if (request.WeightKg.HasValue && request.WeightKg.Value > 0)
                {
                    var weightMatch = candidates
                        .Where(line =>
                            !line.MinWeight.HasValue || request.WeightKg.Value >= line.MinWeight.Value &&
                            !line.MaxWeight.HasValue || request.WeightKg.Value <= line.MaxWeight.Value)
                        .FirstOrDefault();

                    if (weightMatch != null)
                        return weightMatch;
                }

                if (request.VolumeM3.HasValue && request.VolumeM3.Value > 0)
                {
                    var volumeMatch = candidates
                        .Where(line =>
                            !line.MinVolume.HasValue || request.VolumeM3.Value >= line.MinVolume.Value &&
                            !line.MaxVolume.HasValue || request.VolumeM3.Value <= line.MaxVolume.Value)
                        .FirstOrDefault();

                    if (volumeMatch != null)
                        return volumeMatch;
                }

                return candidates.FirstOrDefault();
            }

            private decimal GetContainerPrice(string containerType, TariffLine tariffLine)
            {
                return containerType.ToLower() switch
                {
                    "container20ft" => tariffLine.PricePerContainer20ft ?? 0,
                    "container40ft" => tariffLine.PricePerContainer40ft ?? 0,
                    "reefer20ft" => tariffLine.PricePerContainer20ft ?? 0,
                    "reefer40ft" => tariffLine.PricePerContainer40ft ?? 0,
                    _ => tariffLine.BasePrice ?? 0
                };
            }

            private async Task<(decimal total, List<BaseCompoment> components)> CalculateSurcharges(
                CalculatePriceCommand request,
                decimal basePrice,
                TransportMode transportMode,
                string currencyCode,
                string currencySymbol,
                CancellationToken cancellationToken)
            {
                var components = new List<BaseCompoment>();
                decimal total = 0;

                var surcharges = await _surchargeRepository.GetApplicableSurchargesAsync(
                    transportMode,
                    request.Date ?? DateTime.UtcNow,
                    request.ZoneFromId,
                    request.ZoneToId,
                    cancellationToken);

                foreach (var surcharge in surcharges)
                {
                    decimal amount = surcharge.CalculationType == CalculationType.Percentage
                        ? basePrice * (surcharge.Value / 100)
                        : surcharge.Value;

                    components.Add(new BaseCompoment
                    {
                        Code = surcharge.Code,
                        description = surcharge.Name,
                        amout = amount,
                        currencyCode = currencyCode,
                        calculationbasis = surcharge.CalculationType == CalculationType.Percentage
                            ? $"{basePrice:F2} {currencySymbol} × {surcharge.Value}%"
                            : "Forfait",
                        reference = surcharge.Id.ToString()
                    });

                    total += amount;
                }

                if (!components.Any())
                {
                    var fuelSurcharge = basePrice * 0.10m;
                    components.Add(new BaseCompoment
                    {
                        Code = "SRG_FUEL",
                        description = "Surcharge carburant (10%)",
                        amout = fuelSurcharge,
                        currencyCode = currencyCode,
                        calculationbasis = $"{basePrice:F2} {currencySymbol} × 10%",
                        reference = "FUEL-INDEX"
                    });
                    total += fuelSurcharge;
                }

                return (total, components);
            }

            private async Task<(decimal total, List<BaseCompoment> components)> CalculateTaxes(
                string country,
                decimal amount,
                DateTime date,
                string currencyCode,
                string currencySymbol,
                CancellationToken cancellationToken)
            {
                var components = new List<BaseCompoment>();
                decimal total = 0;

                var taxRule = await _taxRuleRepository.GetApplicableTaxRuleAsync(country, date, cancellationToken);

                if (taxRule != null)
                {
                    var taxAmount = amount * (taxRule.StandardRate / 100);
                    components.Add(new BaseCompoment
                    {
                        Code = taxRule.Code,
                        description = $"{taxRule.Name} ({taxRule.StandardRate}%)",
                        amout = taxAmount,
                        currencyCode = currencyCode,
                        calculationbasis = $"{amount:F2} {currencySymbol} × {taxRule.StandardRate}%",
                        reference = taxRule.Id.ToString()
                    });
                    total = taxAmount;
                }
                else
                {
                    var defaultRate = 20.0m;
                    var taxAmount = amount * (defaultRate / 100);
                    components.Add(new BaseCompoment
                    {
                        Code = "VAT",
                        description = $"TVA par défaut ({defaultRate}%)",
                        amout = taxAmount,
                        currencyCode = currencyCode,
                        calculationbasis = $"{amount:F2} {currencySymbol} × {defaultRate}%",
                        reference = "DEFAULT"
                    });
                    total = taxAmount;
                }

                return (total, components);
            }
        }
    }
}