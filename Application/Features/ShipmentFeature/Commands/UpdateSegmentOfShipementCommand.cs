using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;

namespace Application.Features.ShipmentFeature.Commands
{
    public record UpdateSegmentOfShipementCommand
        (
            Guid ShipmentId,
            Guid SegmentId,
            int? Sequence,
            TransportMode? TransportMode,
            Guid? SupplierId,
            Guid? ZoneFromId,
            Guid? ZoneToId,
            decimal? DistanceKm,
            int? EstimatedTransitDays,
            DateTime? DepartureDate,
            DateTime? ArrivalDate,
            decimal? BaseCost,
            decimal? SurchargesTotal,
            decimal? TotalCost,
            string? CurrencyCode
        ) : IRequest<ResponseHttp>
    {
        public class UpdateSegmentOfShipementCommandHandler : IRequestHandler<UpdateSegmentOfShipementCommand, ResponseHttp>
        {
            private readonly ITransportSegmentRepository _transportSegmentRepository;
            private readonly IMapper _mapper;

            public UpdateSegmentOfShipementCommandHandler(ITransportSegmentRepository transportSegmentRepository, IMapper mapper)
            {
                _transportSegmentRepository = transportSegmentRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdateSegmentOfShipementCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var validationResult = await _transportSegmentRepository.IsExitAsync(request.ShipmentId, request.SegmentId);
                    if (validationResult.ContainsKey(false))
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = validationResult[false]
                        };
                    }

                    var updateSegment = await _transportSegmentRepository.GetByIdAsync(request.SegmentId, cancellationToken);

                    // Use AutoMapper to update only non-null properties
                    _mapper.Map(request, updateSegment);

                    await _transportSegmentRepository.Update(updateSegment);
                    await _transportSegmentRepository.SaveChange(cancellationToken);

                    return new ResponseHttp
                    {
                        Resultat = new TransportSegmentDto(updateSegment),
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
                        Fail_Messages = $"An error occurred while updating the segment: {ex.Message}"
                    };
                }
            }
        }
    }
}
