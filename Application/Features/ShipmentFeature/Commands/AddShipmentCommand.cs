using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

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
            private readonly IMapper mapper;

            public AddShipmentCommandHandler(IShipmentRepository shipementRepository, IMapper mapper)
            {
                this.shipementRepository = shipementRepository;
                this.mapper = mapper;
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

                    // Utilisation d'AutoMapper pour créer l'entité Shipment
                    var shipment = mapper.Map<Shipment>(request);

                    // Définir le statut par défaut (puisqu'il est ignoré dans le mapping)
                    shipment.Status = ShipmentStatus.Draft; // ou le statut par défaut que vous souhaitez

                    shipementRepository.Post(shipment);
                    shipementRepository.SaveChange(cancellationToken);

                    // Utilisation d'AutoMapper pour créer le DTO de réponse
                    var shipmentDto = mapper.Map<ShipmentDto>(shipment);

                    return new ResponseHttp
                    {
                        Resultat = shipmentDto,
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