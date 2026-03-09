using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Queries
{
    public record GetAllCurrenciesQuery:IRequest<ResponseHttp>
    {
        public class GetAllCurreciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;

            public GetAllCurreciesQueryHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
            {
                var currencies = await _currencyRepository.GetAll();
                var currenciesList = currencies?.Select(currency => new CurrencyDto(currency)).ToList();

                return new ResponseHttp
                {
                    Resultat = currenciesList,
                    Status = currenciesList != null && currenciesList.Any() ? 200 : 404,
                    Fail_Messages = currenciesList != null && currenciesList.Any() ? null : "No currencies found."
                };
            }
        }
    }
}
