using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.ShipmentFeature.Commands
{
    public record UpdateShipmentCommand(
    Guid ShipmentId,
    string? ShipmentNumber,
    Guid? ClientId,
    Guid? QuoteId,
    Address? OriginAddress,
    Address? DestinationAddress,
    Guid? MerchandiseTypeId,
    decimal? TotalCostHT,
    decimal? TotalSurcharges,
    decimal? TotalTaxes,
    decimal? TotalCostTTC,
    string? CurrencyCode
    ) : IRequest<ResponseHttp>
    {

        public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, ResponseHttp>
        {

            private readonly IShipmentRepository _shipmentRepository;
            private readonly IMapper _mapper;

            public UpdateShipmentCommandHandler(IShipmentRepository shipmentRepository, IMapper mapper)
            {
                _shipmentRepository = shipmentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipmentToUpdate = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);
                    if (shipmentToUpdate != null)
                    {
                        _mapper.Map(request, shipmentToUpdate); // AutoMapper updates entity
                        await _shipmentRepository.Update(shipmentToUpdate);
                        await _shipmentRepository.SaveChange(cancellationToken);
                        return new ResponseHttp
                        {
                            Status = 200,
                            Resultat = new ShipmentDto(shipmentToUpdate),
                            Fail_Messages = null
                        };
                    }
                    return new ResponseHttp
                    {
                        Status = 404,
                        Resultat = null,
                        Fail_Messages = $"Shipment with {request.ShipmentId} not found."
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Status = 500,
                        Resultat = null,
                        Fail_Messages = $"An error occurred while updating the shipment: {ex.Message}"
                    };
                }
            }
        }
    }
}
