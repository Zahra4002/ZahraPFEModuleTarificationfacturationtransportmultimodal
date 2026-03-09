using Application.Setting;
using Domain.Enums;
using MediatR;

namespace Application.Features.PriceCalculationFeature.Commands
{
    public record CalculateCommand
        (
            Guid ContractId,
            Guid ZoneFromId,
            Guid ZoneToId,
            TransportMode TransportMode,
            decimal weightKg,
            decimal volumeM3,
            ContainerType ContainerType,
            int containerCount,
            Guid merchandiseTypeId,
            DateTime Date
        ) : IRequest<ResponseHttp>
    {
        public class CalculateCommandHandler : IRequestHandler<CalculateCommand, ResponseHttp>
        {




            public async Task<ResponseHttp> Handle(CalculateCommand request, CancellationToken cancellationToken)
            {
                return new ResponseHttp
                {
                    Resultat = new
                    {
                        TotalPrice = 1000m, // Placeholder for actual calculation logic
                        Currency = "USD"
                    },
                    Status = 200,
                    Fail_Messages = null
                };

            }
        }
    }
}