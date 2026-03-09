using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record DeleteCurrencyCommand(
        Guid Id
    ) : IRequest<ResponseHttp>
    {
        public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            public DeleteCurrencyCommandHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
            {
                var currency = await _currencyRepository.GetById(request.Id);
                if (currency == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = "Currency not found"
                    };
                }
                else
                {
                    await _currencyRepository.Delete(request.Id);
                    await _currencyRepository.SaveChange(cancellationToken);
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 204,
                        Fail_Messages = null
                    };
                }
            }
        }
    }
}