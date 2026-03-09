using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
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

            public UpdateSegmentOfShipementCommandHandler(ITransportSegmentRepository transportSegmentRepository)
            {
                _transportSegmentRepository = transportSegmentRepository;
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

                    updateSegment.Sequence = request.Sequence ?? updateSegment.Sequence;
                    updateSegment.TransportMode = request.TransportMode ?? updateSegment.TransportMode;
                    updateSegment.SupplierId = request.SupplierId ?? updateSegment.SupplierId;
                    updateSegment.ZoneFromId = request.ZoneFromId ?? updateSegment.ZoneFromId;
                    updateSegment.ZoneToId = request.ZoneToId ?? updateSegment.ZoneToId;
                    updateSegment.DistanceKm = request.DistanceKm ?? updateSegment.DistanceKm;
                    updateSegment.EstimatedTransitDays = request.EstimatedTransitDays ?? updateSegment.EstimatedTransitDays;
                    updateSegment.DepartureDate = request.DepartureDate ?? updateSegment.DepartureDate;
                    updateSegment.ArrivalDate = request.ArrivalDate ?? updateSegment.ArrivalDate;
                    updateSegment.BaseCost = request.BaseCost ?? updateSegment.BaseCost;
                    updateSegment.SurchargesTotal = request.SurchargesTotal ?? updateSegment.SurchargesTotal;
                    updateSegment.TotalCost = request.TotalCost ?? updateSegment.TotalCost;
                    updateSegment.CurrencyCode = request.CurrencyCode ?? updateSegment.CurrencyCode;

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
