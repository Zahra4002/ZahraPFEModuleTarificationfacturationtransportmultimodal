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
                    // Validate required fields
                    if (!request.ClientId.HasValue)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 400,
                            Fail_Messages = "ClientId is required."
                        };
                    }

                    if (!request.QuoteId.HasValue)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 400,
                            Fail_Messages = "QuoteId is required."
                        };
                    }

                    // Vérification optionnelle du MerchandiseTypeId seulement s'il est fourni
                    if (request.MerchandiseTypeId.HasValue)
                    {
                        var validationResult = await shipementRepository.IsExitAsync(
                            request.ClientId.Value,
                            request.QuoteId.Value,
                            request.MerchandiseTypeId.Value
                        );

                        if (!validationResult.ContainsKey(true))
                        {
                            return new ResponseHttp
                            {
                                Resultat = null,
                                Status = 400,
                                Fail_Messages = validationResult.Values.FirstOrDefault() ?? "Validation failed."
                            };
                        }
                    }
                    else
                    {
                        var clientValidation = await shipementRepository.IsExitAsync(request.ClientId.Value);
                        if (!clientValidation.ContainsKey(true))
                        {
                            return new ResponseHttp
                            {
                                Resultat = null,
                                Status = 400,
                                Fail_Messages = "ClientId does not exist."
                            };
                        }
                    }

                    // Vérification que AutoMapper est configuré
                    if (mapper == null)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 500,
                            Fail_Messages = "AutoMapper is not configured."
                        };
                    }

                    // Utilisation d'AutoMapper pour créer l'entité Shipment
                    var shipment = mapper.Map<Shipment>(request);

                    // Vérifier si le mapping a réussi
                    if (shipment == null)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 500,
                            Fail_Messages = "Failed to map request to Shipment entity."
                        };
                    }

                    // Définir les propriétés qui ne sont pas mappées automatiquement
                    shipment.Id = Guid.NewGuid();
                    shipment.Status = ShipmentStatus.Draft;
                    shipment.CreatedDate = DateTime.UtcNow;
                    shipment.IsDeleted = false;

                    // Initialiser les collections si elles sont null
                    if (shipment.Segments == null)
                        shipment.Segments = new List<TransportSegment>();
                    if (shipment.Invoices == null)
                        shipment.Invoices = new List<Invoice>();

                    // Post the shipment
                    await shipementRepository.Post(shipment);
                    await shipementRepository.SaveChange(cancellationToken);

                    // Utilisation d'AutoMapper pour créer le DTO de réponse
                    var shipmentDto = mapper.Map<ShipmentDto>(shipment);

                    // Vérifier si le mapping de retour a réussi
                    if (shipmentDto == null)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 500,
                            Fail_Messages = "Failed to map Shipment entity to DTO."
                        };
                    }

                    return new ResponseHttp
                    {
                        Resultat = shipmentDto,
                        Status = 200,
                        Fail_Messages = null
                    };
                }
                catch (AutoMapperMappingException amex)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 500,
                        Fail_Messages = $"AutoMapper error: {amex.Message}"
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