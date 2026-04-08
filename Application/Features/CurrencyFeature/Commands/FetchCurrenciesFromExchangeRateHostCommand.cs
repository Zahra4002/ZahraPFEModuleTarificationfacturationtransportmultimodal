using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.CurrencyFeature.Commands
{
    public record FetchCurrenciesFromExchangeRateHostCommand() : IRequest<ResponseHttp>;

    public class FetchCurrenciesFromExchangeRateHostCommandHandler : IRequestHandler<FetchCurrenciesFromExchangeRateHostCommand, ResponseHttp>
    {
        private readonly Application.Services.IExchangeRateHostService _exchangeRateHostService;
        private readonly ICurrencyRepository _currencyRepository;

        public FetchCurrenciesFromExchangeRateHostCommandHandler(
            Application.Services.IExchangeRateHostService exchangeRateHostService,
            ICurrencyRepository currencyRepository)
        {
            _exchangeRateHostService = exchangeRateHostService;
            _currencyRepository = currencyRepository;
        }

        public async Task<ResponseHttp> Handle(FetchCurrenciesFromExchangeRateHostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currencies = await _exchangeRateHostService.FetchAllCurrencies(cancellationToken);
                int importedCount = 0;
                int updatedCount = 0;

                foreach (var currency in currencies)
                {
                    var existingCurrency = await _currencyRepository.SelectOneAsync(c => c.Code == currency.Code, cancellationToken);

                    if (existingCurrency == null)
                    {
                        await _currencyRepository.Post(currency);
                        importedCount++;
                    }
                    else
                    {
                        existingCurrency.Name = currency.Name;
                        existingCurrency.Symbol = currency.Symbol;
                        await _currencyRepository.Update(existingCurrency);
                        updatedCount++;
                    }
                }

                await _currencyRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = new
                    {
                        Message = "Import des devises réussi",
                        CurrenciesImported = importedCount,
                        CurrenciesUpdated = updatedCount,
                        Total = currencies.Count
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de l'import des devises: {ex.Message}"
                };
            }
        }
    }
}