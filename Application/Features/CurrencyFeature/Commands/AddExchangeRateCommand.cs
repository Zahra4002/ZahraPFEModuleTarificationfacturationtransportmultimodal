using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record AddExchangeRateCommand(
        Guid fromCurrencyId,
        Guid toCurrencyId,
        decimal rate,
        DateTime effectiveDate,
        DateTime expiryDate,
        string source
        ) : IRequest<ResponseHttp>
    {
        public class AddExchangeRateCommandHandler : IRequestHandler<AddExchangeRateCommand, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;

            public AddExchangeRateCommandHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }


            public async Task<ResponseHttp> Handle(AddExchangeRateCommand request, CancellationToken cancellationToken)
            {
                var CurrencyFrom = await _currencyRepository.GetById(request.fromCurrencyId);

                if (CurrencyFrom == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency From not found"
                    };
                }

                var CurrencyTo = await _currencyRepository.GetById(request.toCurrencyId);
                if (CurrencyTo == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency To not found"
                    };
                }

                var exchangeRate = new Domain.Entities.ExchangeRate
                {
                    FromCurrencyId = request.fromCurrencyId,
                    ToCurrencyId = request.toCurrencyId,
                    Rate = request.rate,
                    EffectiveDate = request.effectiveDate,
                    ExpiryDate = request.expiryDate,
                    Source = request.source,
                    FromCurrency = CurrencyFrom,
                    ToCurrency = CurrencyTo
                };

                var result = await _currencyRepository.AddExchangeRate(exchangeRate);
                await _currencyRepository.SaveChange(cancellationToken);
                return new ResponseHttp
                {
                    Resultat = new RateDto(exchangeRate),
                    Status = 201,
                    Fail_Messages = "null"
                };
            }

        }
    }
}

