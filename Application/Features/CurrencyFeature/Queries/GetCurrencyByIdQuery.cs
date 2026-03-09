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
    public record  GetCurrencyByIdQuery (Guid Id) : IRequest<ResponseHttp>
    {
        public class GetCurrencyByIdHandler : IRequestHandler<GetCurrencyByIdQuery, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            public GetCurrencyByIdHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }
            public async Task<ResponseHttp> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
            {
                var currency = await _currencyRepository.GetById(request.Id);
                if (currency == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency not found."
                    };
                }
                var currencyDto = new CurrencyDto(currency);
                return new ResponseHttp
                {
                    Resultat = currencyDto,
                    Status = 200,
                    Fail_Messages = null
                };
            }
        }
    }
}
