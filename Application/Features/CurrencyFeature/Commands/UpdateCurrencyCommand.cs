using Application.Features.CurrencyFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using AutoMapper;

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
            private readonly IMapper _mapper;

            public UpdateCurrencyCommandHandler(ICurrencyRepository currencyRepository, IMapper mapper)
            {
                _currencyRepository = currencyRepository;
                _mapper = mapper;
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

                // Utilisation d'AutoMapper pour mapper les données de la commande vers l'entité existante
                _mapper.Map(request, currency);
                
                await _currencyRepository.Update(currency);
                await _currencyRepository.SaveChange(cancellationToken);
                
                // Utilisation d'AutoMapper pour créer le DTO de réponse
                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                
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
