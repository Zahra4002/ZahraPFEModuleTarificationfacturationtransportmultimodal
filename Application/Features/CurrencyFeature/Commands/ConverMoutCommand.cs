using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record ConverMoutCommand
        (
        string FromCurrencyCode,
        string ToCurrencyCode,
        decimal Amount,
        DateTime Date
        ) : IRequest<ResponseHttp>
    {
        public class ConverMoutCommandHandler : IRequestHandler<ConverMoutCommand, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            public ConverMoutCommandHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(ConverMoutCommand request, CancellationToken cancellationToken)
            {
                var rate = await _currencyRepository.ConvertAmount(
                    request.FromCurrencyCode,
                    request.ToCurrencyCode,
                    request.Amount,
                    request.Date,
                    cancellationToken);

                if (rate != null)
                {
                    var convertedAmount = rate.Convert(request.Amount);
                    var result = new
                    {
                        originalAmount = request.Amount,
                        fromCurrencyCode = request.FromCurrencyCode,
                        convertedAmount = convertedAmount,
                        toCurrencyCode = request.ToCurrencyCode,
                        rateUsed = rate.Rate,
                        rateDate = rate.EffectiveDate
                    };

                    return new ResponseHttp
                    {
                        Resultat = result,
                        Status = 200,
                        Fail_Messages = null
                    };
                }
                else
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Conversion failed. Please check the currency codes and date."
                    };
                }
            }
        }
    }
}
