using Application.Extensions;
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
            private readonly IMapper _mapper;

            public CalculateCommandHandler(
                IClientRepository clientRepository,
                IContractRepository contractRepository,
                ITariffGridRepository tariffRepository,
                ISurchargeRepository surchargeRepository,
                ITaxRuleRepository taxRuleRepository,
                IZoneRepository zoneRepository,
                IMapper mapper)
            {
                _clientRepository = clientRepository;
                _contractRepository = contractRepository;
                _tariffRepository = tariffRepository;
                _surchargeRepository = surchargeRepository;
                _taxRuleRepository = taxRuleRepository;
                _zoneRepository = zoneRepository;
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
                    
                    // Déterminer le mode de transport (accepte français et enum)
                    if (!TransportModeExtensions.TryParseTransportMode(request.TransportMode, out var transportMode))
                    {
                        return new ResponseHttp
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Fail_Messages = $"Mode de transport invalide: {request.TransportMode}. Modes acceptés: Maritime, Aérien, Routier, Ferroviaire, Fluvial"
                        };
                    }

                    // 3️⃣ Calculer le prix de base
                    var (basePrice, baseComponents) = await CalculateBasePrice(
                        request, transportMode, client, cancellationToken);

                    // 4️⃣ Calculer les surcharges
                    var (surchargesTotal, surchargeComponents) = await CalculateSurcharges(
                        request, basePrice, transportMode, cancellationToken);

                    var subtotal = basePrice + surchargesTotal;

                    // 5️⃣ Calculer les taxes
                    var (taxTotal, taxComponents) = await CalculateTaxes(
                        destinationCountry, subtotal, request.Date ?? DateTime.UtcNow, cancellationToken);

                    var totalCost = subtotal + taxTotal;

                    // 6️⃣ Déterminer la devise
                    var currencyCode = client?.DefaultCurrencyCode ?? "EUR";

                    // 7️⃣ Construire le résultat détaillé
                    var result = new ResultDto
                    {
                        baseCost = Math.Round(basePrice, 2),
                        surchargesTotal = Math.Round(surchargesTotal, 2),
                        subtotal = Math.Round(subtotal, 2),
                        taxTotal = Math.Round(taxTotal, 2),
                        currencyCode = currencyCode,
                        breakDown = new BreakDown
                        {
                            baseCompoments = baseComponents,
                            Surcharges = surchargeComponents,
                            Taxes = taxComponents
                        },
                        appliedContractNumber = contract?.ContractNumber ?? "N/A",
                        appliedTariffGrid = await GetAppliedTariffGridCode(request, transportMode, cancellationToken),
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
                TransportMode transportMode,
                Client? client,
                CancellationToken cancellationToken)
            {
                var components = new List<BaseCompoment>();
                decimal total = 0;

                var tariffGrid = await _tariffRepository.GetApplicableTariffGridAsync(
                    transportMode, 
                    request.Date ?? DateTime.UtcNow, 
                    cancellationToken);

                string tariffCode = tariffGrid?.Code ?? "TARIF-STANDARD";

                if (request.WeightKg.HasValue && request.WeightKg.Value > 0)
                {
                    var ratePerKg = 2.5m;
                    var amount = request.WeightKg.Value * ratePerKg;
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_WT",
                        description = $"Prix base au poids ({request.WeightKg:F0} kg x {ratePerKg:F2} €/kg)",
                        amout = amount,
                        currencyCode = "EUR",
                        calculationbasis = $"{request.WeightKg:F0} kg",
                        reference = tariffCode
                    });
                    total += amount;
                }

                if (request.VolumeM3.HasValue && request.VolumeM3.Value > 0 && total == 0)
                {
                    var ratePerM3 = 50m;
                    var amount = request.VolumeM3.Value * ratePerM3;
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_VOL",
                        description = $"Prix base au volume ({request.VolumeM3:F2} m³ x {ratePerM3:F2} €/m³)",
                        amout = amount,
                        currencyCode = "EUR",
                        calculationbasis = $"{request.VolumeM3:F2} m³",
                        reference = tariffCode
                    });
                    total += amount;
                }

                if (!string.IsNullOrEmpty(request.ContainerType) && request.ContainerCount.HasValue)
                {
                    var pricePerContainer = GetPricePerContainer(request.ContainerType);
                    var amount = pricePerContainer * request.ContainerCount.Value;
                    components.Add(new BaseCompoment
                    {
                        Code = "BASE_CNT",
                        description = $"Prix base conteneurs ({request.ContainerCount} x {request.ContainerType})",
                        amout = amount,
                        currencyCode = "EUR",
                        calculationbasis = $"{request.ContainerCount} conteneurs",
                        reference = tariffCode
                    });
                    total += amount;
                }

                return (total, components);
            }

            private decimal GetPricePerContainer(string containerType)
            {
                return containerType.ToLower() switch
                {
                    "container20ft" => 1000m,
                    "container40ft" => 1800m,
                    "reefer20ft" => 1500m,
                    "reefer40ft" => 2500m,
                    _ => 1000m
                };
            }

            private async Task<(decimal total, List<BaseCompoment> components)> CalculateSurcharges(
                CalculatePriceCommand request,
                decimal basePrice,
                TransportMode transportMode,
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
                        currencyCode = "EUR",
                        calculationbasis = surcharge.CalculationType == CalculationType.Percentage
                            ? $"{basePrice:F2} € × {surcharge.Value}%"
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
                        currencyCode = "EUR",
                        calculationbasis = $"{basePrice:F2} € × 10%",
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
                        currencyCode = "EUR",
                        calculationbasis = $"{amount:F2} € × {taxRule.StandardRate}%",
                        reference = taxRule.Id.ToString()
                    });
                    total += taxAmount;
                }
                else
                {
                    var defaultRate = 19.0m;
                    var taxAmount = amount * (defaultRate / 100);
                    components.Add(new BaseCompoment
                    {
                        Code = "VAT",
                        description = $"TVA par défaut ({defaultRate}%)",
                        amout = taxAmount,
                        currencyCode = "EUR",
                        calculationbasis = $"{amount:F2} € × {defaultRate}%",
                        reference = "DEFAULT"
                    });
                    total += taxAmount;
                }

                return (total, components);
            }

            private async Task<string> GetAppliedTariffGridCode(CalculatePriceCommand request, TransportMode transportMode, CancellationToken cancellationToken)
            {
                var tariffGrid = await _tariffRepository.GetApplicableTariffGridAsync(
                    transportMode, 
                    request.Date ?? DateTime.UtcNow, 
                    cancellationToken);

                return tariffGrid?.Code ?? "TARIF-STANDARD";
            }
        }
    }
}