using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Commands
{
    public record RecalculateShipmentCommandcs(Guid ShipmentId) : IRequest<ResponseHttp>
    {
        public class RecalculateShipmentCommandcsHandler : IRequestHandler<RecalculateShipmentCommandcs, ResponseHttp>
        {
            private readonly IShipmentRepository _shipmentRepository;

            public RecalculateShipmentCommandcsHandler(IShipmentRepository shipmentRepository)
            {
                _shipmentRepository = shipmentRepository;
            }

            public async Task<ResponseHttp> Handle(RecalculateShipmentCommandcs request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipmentToRecalculate = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);

                    if (shipmentToRecalculate != null)
                    {
                        shipmentToRecalculate.RecalculateTotals();
                        await _shipmentRepository.Update(shipmentToRecalculate);
                        await _shipmentRepository.SaveChange(cancellationToken);

                        return new ResponseHttp
                        {
                            Resultat = new ShipmentDto(shipmentToRecalculate),
                            Status = 200,
                            Fail_Messages = null
                        };
                    }

                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = $"Shipment with ID {request.ShipmentId} not found."
                    };

                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 500,
                        Fail_Messages = $"An error occurred while recalculating the shipment: {ex.Message}"
                    };

                }
            }
        }
    }
}
