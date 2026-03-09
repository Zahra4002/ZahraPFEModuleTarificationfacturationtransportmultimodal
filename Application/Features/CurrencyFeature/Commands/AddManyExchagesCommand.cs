using Application.Setting;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record AddManyExchagesCommand(
        List<AddExchangeRateCommand> ExchangeRates
        ) : IRequest<ResponseHttp>
    {
        public class AddManyExchagesCommandHandler : IRequestHandler<AddManyExchagesCommand, ResponseHttp>
        {
            private readonly IMediator _mediator;
            public AddManyExchagesCommandHandler(IMediator mediator)
            {
                _mediator = mediator;
            }
            public async Task<ResponseHttp> Handle(AddManyExchagesCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    foreach (var item in request.ExchangeRates)
                    {
                        var result = await _mediator.Send(item, cancellationToken);
                        // Accept both 200 and 201 as success
                        if (result.Status != 200 && result.Status != 201)
                        {
                            return new ResponseHttp
                            {
                                Resultat = null,
                                Status = result.Status,
                                Fail_Messages = $"Failed to add exchange rate for {item.fromCurrencyId} to {item.toCurrencyId}: {result.Fail_Messages}"
                            };
                        }
                    }
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 200,
                        Fail_Messages = null
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 500,
                        Fail_Messages = $"An error occurred while adding exchange rates: {ex.Message}"
                    };
                }
            }
        }
    }
}
