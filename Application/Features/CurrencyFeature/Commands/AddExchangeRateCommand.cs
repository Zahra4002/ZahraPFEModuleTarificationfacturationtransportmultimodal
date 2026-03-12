using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using AutoMapper;

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
            private readonly IMapper _mapper;

            public AddExchangeRateCommandHandler(ICurrencyRepository currencyRepository, IMapper mapper)
            {
                _currencyRepository = currencyRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddExchangeRateCommand request, CancellationToken cancellationToken)
            {
                var currencyFrom = await _currencyRepository.GetById(request.fromCurrencyId);
                if (currencyFrom == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency From not found"
                    };
                }

                var currencyTo = await _currencyRepository.GetById(request.toCurrencyId);
                if (currencyTo == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency To not found"
                    };
                }

                // Utilisation d'AutoMapper pour créer l'ExchangeRate
                var exchangeRate = _mapper.Map<ExchangeRate>(request);
                exchangeRate.Id = Guid.NewGuid();
                exchangeRate.FromCurrency = currencyFrom;
                exchangeRate.ToCurrency = currencyTo;

                var result = await _currencyRepository.AddExchangeRate(exchangeRate);
                await _currencyRepository.SaveChange(cancellationToken);

                // Utilisation d'AutoMapper pour créer le DTO
                var rateDto = _mapper.Map<RateDto>(exchangeRate);

                return new ResponseHttp
                {
                    Resultat = rateDto,
                    Status = 201,
                    Fail_Messages = null
                };
            }
        }
    }
}

