using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetShipmentDetailsById(Guid ShipmentId): IRequest<ResponseHttp>
    {

       public class GetShipementDetailsByIdHandler : IRequestHandler<GetShipmentDetailsById, ResponseHttp>
        {
            private readonly IShipmentRepository _shipmentRepository;
            public GetShipementDetailsByIdHandler(IShipmentRepository shipmentRepository)
            {
                _shipmentRepository = shipmentRepository;
            }
            public async Task<ResponseHttp> Handle(GetShipmentDetailsById request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipmentDetails = await _shipmentRepository.GetByIdAsync(request.ShipmentId,cancellationToken);
                    if (shipmentDetails != null)
                    {
                        return new ResponseHttp
                        {
                            Status = 200,
                            Fail_Messages = null,
                            Resultat = shipmentDetails
                        };
                    }
                    else
                    {
                        return new ResponseHttp
                        {
                            Status = 404,
                            Fail_Messages = $"Shipment wih ID:{request.ShipmentId} not found.",
                            Resultat = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not implemented here)
                    return new ResponseHttp
                    {
                        Status = 500,
                        Fail_Messages = $"An error occurred while retrieving shipment details: {ex.Message}",
                        Resultat = null
                    };
                }
            }
        }
    }
}
