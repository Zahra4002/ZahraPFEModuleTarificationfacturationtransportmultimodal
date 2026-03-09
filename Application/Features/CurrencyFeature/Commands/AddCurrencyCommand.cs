using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;

namespace Application.Features.CurrencyFeature.Commands
{
    public record AddCurrencyCommand(
        string Code,
        string Name,
        string Symbol,
        int DecimalPlaces,
        bool IsActive,
        bool IsDefault
        ) : IRequest<ResponseHttp>
    {
        public class AddCurrencyCommandHandler : IRequestHandler<AddCurrencyCommand, ResponseHttp>
        {
            private readonly ICurrencyRepository _currencyRepository;
            public AddCurrencyCommandHandler(ICurrencyRepository currencyRepository)
            {
                _currencyRepository = currencyRepository;
            }
            public async Task<ResponseHttp> Handle(AddCurrencyCommand request, CancellationToken cancellationToken)
            {

                var newCurrency = new Currency
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code,
                    Name = request.Name,
                    Symbol = request.Symbol,
                    DecimalPlaces = request.DecimalPlaces,
                    IsActive = request.IsActive,
                    IsDefault = request.IsDefault
                };

                await _currencyRepository.Post(newCurrency);
                await _currencyRepository.SaveChange(cancellationToken);
                CurrencyDto currencyDto = new(newCurrency);
                return new ResponseHttp
                {
                    Resultat = currencyDto, 
                    Status = 201, 
                    Fail_Messages = "Currency created successfully"
                };
            }
        }
    }
}
