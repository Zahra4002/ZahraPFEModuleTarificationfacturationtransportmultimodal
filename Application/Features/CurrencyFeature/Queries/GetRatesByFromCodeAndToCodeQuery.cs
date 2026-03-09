using Application.Interfaces;
using Application.Setting;
using MediatR;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Queries
{
    public record GetRatesByFromCodeAndToCodeQuery
        (
        string FromCurrencyCode,
        string ToCurrencyCode,
        DateTime? DateFrom = null,
        DateTime? DateTo = null
        ) : IRequest<ResponseHttp>
    {
        public class GetRatesByFromCodeAndToCodeQueryHandler : IRequestHandler<GetRatesByFromCodeAndToCodeQuery, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            public GetRatesByFromCodeAndToCodeQueryHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }
            public async Task<ResponseHttp> Handle(GetRatesByFromCodeAndToCodeQuery request, CancellationToken cancellationToken)
            {
                var rates = await _currencyRepository.GetRatesByFromCodeAndToCode(request.FromCurrencyCode, request.ToCurrencyCode, request.DateFrom, request.DateTo, cancellationToken);
                return new ResponseHttp
                {
                    Resultat = rates.Select(rate => new Dtos.RateDto(rate)),
                    Status = rates != null && rates.Any() ? 200 : 404,
                    Fail_Messages = rates != null && rates.Any() ? null : "No exchange rates found for the specified currency codes."
                };
            }


        }
    }
}
