using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetShipmentById(Guid ShipmentId) : IRequest<ResponseHttp>
    {

        public class GetShipmentByIdHandler : IRequestHandler<GetShipmentById, ResponseHttp>
        {
            private readonly IShipmentRepository _shipementRepository;
            public GetShipmentByIdHandler(IShipmentRepository shipementRepository)
            {
                _shipementRepository = shipementRepository;
            }
            public async Task<ResponseHttp> Handle(GetShipmentById request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipment = await _shipementRepository.GetShipementWithIncules(request.ShipmentId, new string[] { "Segments" }, cancellationToken);
                    
                    if (shipment == null)
                    {
                        return new ResponseHttp
                        {
                            Status = 404,
                            Fail_Messages = $"Shipment with ID {request.ShipmentId} not found."
                        };
                    }

                    var result = new ShipmentDto(shipment);
                    
                    return new ResponseHttp
                    {
                        Status = 200,
                        Resultat = result,
                        Fail_Messages = null
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Status = 500,
                        Fail_Messages = $"An error occurred while retrieving the shipment: {ex.Message}"
                    };
                }

            }
        }
    }
}
