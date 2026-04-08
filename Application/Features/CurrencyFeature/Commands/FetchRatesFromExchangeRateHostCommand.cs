using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.CurrencyFeature.Commands
{
    public record FetchRatesFromExchangeRateHostCommand(
        string BaseCurrency = "EUR"
    ) : IRequest<ResponseHttp>;

    public class FetchRatesFromExchangeRateHostCommandHandler : IRequestHandler<FetchRatesFromExchangeRateHostCommand, ResponseHttp>
    {
        private readonly Application.Services.IExchangeRateHostService _exchangeRateHostService;
        private readonly ICurrencyRepository _currencyRepository;

        public FetchRatesFromExchangeRateHostCommandHandler(
            Application.Services.IExchangeRateHostService exchangeRateHostService,
            ICurrencyRepository currencyRepository)
        {
            _exchangeRateHostService = exchangeRateHostService;
            _currencyRepository = currencyRepository;
        }

        public async Task<ResponseHttp> Handle(FetchRatesFromExchangeRateHostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rates = await _exchangeRateHostService.FetchAllRates(request.BaseCurrency, cancellationToken);

                if (rates.Count == 0)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Aucun taux trouvé"
                    };
                }

                foreach (var rate in rates)
                {
                    await _currencyRepository.AddExchangeRate(rate);
                }

                await _currencyRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = new
                    {
                        Message = "Import réussi",
                        RatesImported = rates.Count,
                        BaseCurrency = request.BaseCurrency
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de l'import: {ex.Message}"
                };
            }
        }
    }
}