using Application.Interfaces;
using Application.Services;
using Application.Setting;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TaxFeature.Commands
{
    public record FetchTaxRatesCommand() : IRequest<ResponseHttp>;

    public class FetchTaxRatesCommandHandler : IRequestHandler<FetchTaxRatesCommand, ResponseHttp>
    {
        private readonly IVatRateService _vatRateService;
        private readonly ITaxRuleRepository _taxRuleRepository;

        public FetchTaxRatesCommandHandler(
            IVatRateService vatRateService,
            ITaxRuleRepository taxRuleRepository)
        {
            _vatRateService = vatRateService;
            _taxRuleRepository = taxRuleRepository;
        }

        public async Task<ResponseHttp> Handle(FetchTaxRatesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var vatRates = await _vatRateService.GetAllVatRates(cancellationToken);

                if (vatRates.Count == 0)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Aucun taux de TVA trouvé"
                    };
                }

                int importedCount = 0;
                int updatedCount = 0;

                foreach (var vat in vatRates)
                {
                    var existing = await _taxRuleRepository.GetByCountryCode(vat.CountryCode, cancellationToken);

                    if (existing == null)
                    {
                        var taxRule = new TaxRule
                        {
                            Id = Guid.NewGuid(),
                            Code = vat.CountryCode,
                            Name = $"TVA {vat.CountryName}",
                            Country = vat.CountryName,
                            CountryCode = vat.CountryCode,
                            StandardRate = vat.StandardRate,
                            ReducedRate = vat.ReducedRate,
                            IsActive = true,
                            ValidFrom = DateTime.UtcNow,
                            ValidTo = null,
                            CreatedDate = DateTime.UtcNow
                        };
                        await _taxRuleRepository.Post(taxRule);
                        importedCount++;
                    }
                    else
                    {
                        existing.StandardRate = vat.StandardRate;
                        existing.ReducedRate = vat.ReducedRate;
                        existing.ModifiedDate = DateTime.UtcNow;
                        await _taxRuleRepository.Update(existing);
                        updatedCount++;
                    }
                }

                await _taxRuleRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = new
                    {
                        Message = "Import des taux de TVA réussi",
                        Imported = importedCount,
                        Updated = updatedCount,
                        Total = vatRates.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de l'import des TVA: {ex.Message}"
                };
            }
        }
    }
}