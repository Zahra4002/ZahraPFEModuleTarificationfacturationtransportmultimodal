using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Intuit.Ipp.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Queries
{
    public record GetDefaultCurrenciesQuery(bool isDefault):IRequest<ResponseHttp>
    {
        public class GetDefaultCurrenciesQueryHandler:IRequestHandler<GetDefaultCurrenciesQuery,ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;

            public GetDefaultCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(GetDefaultCurrenciesQuery request, CancellationToken cancellationToken)
            {
                var currencies =await  _currencyRepository.SelectManyAsync(cs => cs.IsDefault ==request.isDefault, cancellationToken);

                return new ResponseHttp
                {
                    Resultat = currencies.Select(c => new CurrencyDto (c)),
                    Status = 200,
                    Fail_Messages = null
                };
            }
        }
     }
}
