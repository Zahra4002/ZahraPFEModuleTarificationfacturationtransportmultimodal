using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.ValueObjects;
using MediatR;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public UpdateShipmentCommandHandler(IShipmentRepository shipmentRepository)
            {
                _shipmentRepository = shipmentRepository;
            }
            public async Task<ResponseHttp> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
            {
                 
                try
                {

                    var shipmentToUpdate = await _shipmentRepository.GetByIdAsync(request.ShipmentId,cancellationToken);
                    if (shipmentToUpdate != null)
                    {
                        shipmentToUpdate.ShipmentNumber = request.ShipmentNumber ?? shipmentToUpdate.ShipmentNumber;
                        shipmentToUpdate.ClientId = request.ClientId ?? shipmentToUpdate.ClientId;
                        shipmentToUpdate.QuoteId = request.QuoteId ?? shipmentToUpdate.QuoteId;
                        shipmentToUpdate.OriginAddress = request.OriginAddress ?? shipmentToUpdate.OriginAddress;
                        shipmentToUpdate.DestinationAddress = request.DestinationAddress ?? shipmentToUpdate.DestinationAddress;
                        shipmentToUpdate.MerchandiseTypeId = request.MerchandiseTypeId ?? shipmentToUpdate.MerchandiseTypeId;
                        shipmentToUpdate.TotalCostHT = request.TotalCostHT ?? shipmentToUpdate.TotalCostHT;
                        shipmentToUpdate.TotalSurcharges = request.TotalSurcharges ?? shipmentToUpdate.TotalSurcharges;
                        shipmentToUpdate.TotalTaxes = request.TotalTaxes ?? shipmentToUpdate.TotalTaxes;
                        shipmentToUpdate.TotalCostTTC = request.TotalCostTTC ?? shipmentToUpdate.TotalCostTTC;
                        shipmentToUpdate.CurrencyCode = request.CurrencyCode ?? shipmentToUpdate.CurrencyCode;
                        await _shipmentRepository.Update(shipmentToUpdate);
                        await _shipmentRepository.SaveChange(cancellationToken);
                        return new ResponseHttp
                        {
                            Status = 200,
                            Resultat = new ShipmentDto( shipmentToUpdate),
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
