using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.ShipmentFeature.Commands
{
    public record DeleteShipmentCommand(Guid Id) : IRequest<ResponseHttp>
    {
        public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, ResponseHttp>
        {
            private readonly IShipmentRepository _shipmentRepository;
            public DeleteShipmentCommandHandler(IShipmentRepository shipmentRepository)
            {
                _shipmentRepository = shipmentRepository;
            }
            public async Task<ResponseHttp> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
            {
                var shipment = await _shipmentRepository.GetByIdAsync(request.Id, cancellationToken);
                if (shipment == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = $"Shipment with Id : {request.Id} not found."
                    };
                }
                await _shipmentRepository.Delete(request.Id);
                await _shipmentRepository.SaveChange(cancellationToken);
                return new ResponseHttp
                {
                    Resultat = new ShipmentDto(shipment),
                    Status = 200,
                    Fail_Messages = $"Shipment with Id : {request.Id} deleted successfully."
                };
            }
        }
    }
}
