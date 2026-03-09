using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetShipmentsByStatus(Domain.Enums.ShipmentStatus Status) : IRequest<ResponseHttp>
    {
        public class GetShipmentsByStatusHandler : IRequestHandler<GetShipmentsByStatus, ResponseHttp>
        {
            private readonly IShipmentRepository _repository;
            public GetShipmentsByStatusHandler(IShipmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<ResponseHttp> Handle(GetShipmentsByStatus request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipments = await _repository.SelectManyAsync(s => s.Status == request.Status, cancellationToken);
                    if (shipments == null || shipments.Count == 0)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = "No Shipments found with the specified status."
                        };
                    }
                    return new ResponseHttp
                    {
                        Resultat = shipments.Select(s => new ShipmentDto(s)).ToList(),
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
                        Fail_Messages = $"An error occurred while retrieving shipments: {ex.Message}"
                    };
                }
            }
        }

    }
}
