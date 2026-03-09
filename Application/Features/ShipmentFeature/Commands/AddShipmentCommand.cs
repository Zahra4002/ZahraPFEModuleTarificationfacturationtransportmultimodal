using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Commands
{
    public record AddShipmentCommand
        (
        string ShipmentNumber,
        Guid? ClientId,
        Guid? QuoteId,
        AddressDto OriginAddress,
        AddressDto DestinationAddress,
        Guid? MerchandiseTypeId,
        decimal? WeightKg,
        decimal? VolumeM3,
        ContainerType? ContainerType,
        int? ContainerCount,
        decimal TotalCostHT,
        decimal TotalSurcharges,
        decimal TotalTaxes,
        decimal TotalCostTTC,
        string CurrencyCode


        ) : IRequest<ResponseHttp>

    {

        public class AddShipmentCommandHandler : IRequestHandler<AddShipmentCommand, ResponseHttp>
        {
            private readonly IShipmentRepository shipementRepository;
            public AddShipmentCommandHandler(IShipmentRepository shipementRepository)
            {
                this.shipementRepository = shipementRepository;
            }


            async Task<ResponseHttp> IRequestHandler<AddShipmentCommand, ResponseHttp>.Handle(AddShipmentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // Perform validation to check if a shipment with the same ClientId, QuoteId, and MerchandiseTypeId already exists
                    var validationResult = await shipementRepository.IsExitAsync(
                        request.ClientId.Value,
                        request.QuoteId.Value,
                        request.MerchandiseTypeId.Value
                    );
                    // If validation fails, return a 400 Bad Request response with the validation error message
                    if (!validationResult.ContainsKey(true))
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 400,
                            Fail_Messages = validationResult.Values.FirstOrDefault() ?? "Validation failed."
                        };
                    }

                    var shipment = new Shipment
                    {
                        ShipmentNumber = request.ShipmentNumber,
                        ClientId = request.ClientId,
                        QuoteId = request.QuoteId,
                        OriginAddress = new Address
                        {
                            Street = request.OriginAddress.Street,
                            City = request.OriginAddress.City,
                            State = request.OriginAddress.State,
                            PostalCode = request.OriginAddress.PostalCode,
                            Country = request.OriginAddress.Country
                        },
                        DestinationAddress =  new Address
                        {
                            Street = request.DestinationAddress.Street,
                            City = request.DestinationAddress.City,
                            State = request.DestinationAddress.State,
                            PostalCode = request.DestinationAddress.PostalCode,
                            Country = request.DestinationAddress.Country
                        },
                        MerchandiseTypeId = request.MerchandiseTypeId,
                        WeightKg = request.WeightKg,
                        VolumeM3 = request.VolumeM3,
                        ContainerType = request.ContainerType,
                        ContainerCount = request.ContainerCount,
                        TotalCostHT = request.TotalCostHT,
                        TotalSurcharges = request.TotalSurcharges,
                        TotalTaxes = request.TotalTaxes,
                        TotalCostTTC = request.TotalCostTTC,
                        CurrencyCode = request.CurrencyCode
                    };
                    shipementRepository.Post(shipment);
                    shipementRepository.SaveChange(cancellationToken);
                    return new ResponseHttp
                    {
                        Resultat = new ShipmentDto(shipment),
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
                            Fail_Messages = $"An error occurred while adding the shipment: {ex.Message}"
                        };

                }
            }
        }
    }
}
