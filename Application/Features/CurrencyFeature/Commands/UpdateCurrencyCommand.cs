using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record UpdateCurrencyCommand(
        Guid Id,
        string Code,
        string Name,
        string Symbol,
        int DecimalPlaces,
        bool IsActive,
        bool IsDefault
        ) : IRequest<ResponseHttp>
    {
        public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, ResponseHttp>
        {

            private readonly ICurrencyRepository _currencyRepository;

            public UpdateCurrencyCommandHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }

            public async Task<ResponseHttp> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
            {
                Currency? currency = await _currencyRepository.GetByIdAsync(request.Id, cancellationToken);
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

                    currency.Code = request.Code;
                    currency.Name = request.Name;
                    currency.Symbol = request.Symbol;
                    currency.DecimalPlaces = request.DecimalPlaces;
                    currency.IsActive = request.IsActive;
                    currency.IsDefault = request.IsDefault;
                    await _currencyRepository.Update(currency);
                    await _currencyRepository.SaveChange(cancellationToken);
                    return new ResponseHttp
                    {
                        Resultat = new CurrencyDto(currency),
                        Status = 200,
                        Fail_Messages = null
                    };

                }

            }
        }
    }
}
