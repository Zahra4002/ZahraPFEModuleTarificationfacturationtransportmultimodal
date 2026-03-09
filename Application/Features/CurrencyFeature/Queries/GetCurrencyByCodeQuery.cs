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
    public record GetCurrencyByCodeQuery(string code):IRequest<ResponseHttp>
    {
        public class GetCurrencyByCodeQueryHanler : IRequestHandler<GetCurrencyByCodeQuery,ResponseHttp>
        {
            private readonly ICurrencyRepository currencyRepository; 

            public GetCurrencyByCodeQueryHanler(ICurrencyRepository currencyRepository)
            {
                this.currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(GetCurrencyByCodeQuery request, CancellationToken cancellationToken)
            {
                var currency = await currencyRepository.SelectOneAsync(c => c.Code==request.code.ToUpper(), cancellationToken);

                if (currency == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = $"Currency with code '{request.code}' not found."
                    };
                }
                else
                {
                                        return new ResponseHttp
                    {
                        Resultat =  new CurrencyDto(currency),
                        Status = 200,
                        Fail_Messages = null
                    };
                }
            }
        }
    }
}
