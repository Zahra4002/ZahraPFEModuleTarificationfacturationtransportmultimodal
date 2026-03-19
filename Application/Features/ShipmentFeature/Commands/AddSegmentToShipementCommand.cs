using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using AutoMapper;

namespace Application.Features.ShipmentFeature.Commands
{
    public record AddSegmentToShipementCommand

        (
           Guid ShipmentId,
           TransportMode TransportMode,
           Guid SupplierId,
           Guid ZoneFromId,
           Guid ZoneToId,
           int sequence,
           decimal DistanceKm,
           int EstimatedTransitDays,
           DateTime? DepartureDate,
           DateTime? ArrivalDate,
           decimal BaseCost,
           decimal SurchargesTotal,
           decimal TotalCost,
           string CurrencyCode
        ) : IRequest<ResponseHttp>
    {
        public class AddSegmentToShipementCommandHandler : IRequestHandler<AddSegmentToShipementCommand, ResponseHttp>
        {
            private readonly IShipmentRepository _shipmentRepository;
            private readonly ITransportSegmentRepository _transportSegmentRepository;
            private readonly IMapper _mapper;

            public AddSegmentToShipementCommandHandler(
                IShipmentRepository shipmentRepository,
                ITransportSegmentRepository transportSegmentRepository,
                IMapper mapper)
            {
                _shipmentRepository = shipmentRepository;
                _transportSegmentRepository = transportSegmentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddSegmentToShipementCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var validationResult = await _transportSegmentRepository.IsExitAsync(request.ShipmentId, request.SupplierId, request.ZoneFromId, request.ZoneToId);

                    // Check for missing entity (Shipment, Supplier, Zone)
                    var failMessage = validationResult.Values.FirstOrDefault();
                    if (validationResult.ContainsKey(false) && failMessage != "Transport Segment does not exist and can be created.")
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = failMessage ?? "Resource not found."
                        };
                    }

                    // Segment already exists
                    if (validationResult.ContainsKey(true))
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 400,
                            Fail_Messages = failMessage ?? "Transport Segment already exists."
                        };
                    }

                    // Use AutoMapper to create the segment
                    var segment = _mapper.Map<TransportSegment>(request);

                    await _transportSegmentRepository.Post(segment);
                    await _transportSegmentRepository.SaveChange(cancellationToken);

                    return new ResponseHttp
                    {
                        Resultat = new TransportSegmentDto(segment),
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
                        Fail_Messages = $"An error occurred while adding the segment to the shipment: {ex.Message}"
                    };
                }
            }

        }
    }
}

