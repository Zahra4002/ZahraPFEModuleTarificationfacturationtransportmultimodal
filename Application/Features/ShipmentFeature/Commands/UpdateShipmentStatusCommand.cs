using Application.Interfaces;
using Application.Setting;
using Domain.Enums;
using Intuit.Ipp.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Commands
{
    public record UpdateShipmentStatusCommand(Guid ShipmentId, ShipmentStatus Status) : IRequest<ResponseHttp>
    {
        public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, ResponseHttp>
        {
            private readonly IShipmentRepository _shipmentRepository;
            public UpdateShipmentStatusCommandHandler(IShipmentRepository shipmentRepository)
            {
                _shipmentRepository = shipmentRepository;
            }
            public async Task<ResponseHttp> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipmentDetails = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
                    if (shipmentDetails==null)
                    {
                        return new ResponseHttp
                        {
                            Status = 404,
                            Fail_Messages = $"Shipement with ID:{request.ShipmentId} not found ",
                            Resultat = null
                        };
                    }
                    else
                    {
                        var previousStatus = shipmentDetails.Status;
                        shipmentDetails.Status = request.Status;
                        await _shipmentRepository.Update(shipmentDetails);
                        await _shipmentRepository.SaveChange(cancellationToken);
                        return new ResponseHttp
                        {
                            Status = 200,
                            Fail_Messages = $"Shipment with ID:{request.ShipmentId} Status modified From {previousStatus} to {request.Status}",
                            Resultat = new { Guid = shipmentDetails.Id, StatusFrom= previousStatus,StatusTo = shipmentDetails.Status }
                        };
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (not implemented here)
                    return new ResponseHttp
                    {
                        Status = 500,
                        Fail_Messages = $"An error occurred while updating shipment status: {ex.Message}",
                        Resultat = null
                    };
                }
            }
        }
    }
}
