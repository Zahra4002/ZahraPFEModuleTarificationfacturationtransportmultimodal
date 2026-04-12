using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
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
            string? CurrencyCode,
            SegmentStatus? Status
        ) : IRequest<ResponseHttp>;

    public class UpdateSegmentOfShipementCommandHandler : IRequestHandler<UpdateSegmentOfShipementCommand, ResponseHttp>
    {
        private readonly ITransportSegmentRepository _transportSegmentRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public UpdateSegmentOfShipementCommandHandler(
            ITransportSegmentRepository transportSegmentRepository,
            IShipmentRepository shipmentRepository,
            IMapper mapper)
        {
            _transportSegmentRepository = transportSegmentRepository;
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateSegmentOfShipementCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validation: vérifier que le segment existe et appartient au shipment
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

                // Récupérer le segment existant
                var updateSegment = await _transportSegmentRepository.GetByIdAsync(request.SegmentId, cancellationToken);
                if (updateSegment == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 404,
                        Fail_Messages = $"Segment with ID {request.SegmentId} not found."
                    };
                }

                // Mettre à jour le statut si fourni
                if (request.Status.HasValue)
                {
                    updateSegment.Status = request.Status.Value;
                    Console.WriteLine($"Updating segment {request.SegmentId} status to: {request.Status.Value}");
                }

                // Mapper les autres propriétés (sans écraser le statut)
                _mapper.Map(request, updateSegment);

                // Sauvegarder les modifications
                await _transportSegmentRepository.Update(updateSegment);
                await _transportSegmentRepository.SaveChange(cancellationToken);

                // Mettre à jour la progression du shipment si le statut a changé
                if (request.Status.HasValue)
                {
                    await UpdateShipmentProgress(request.ShipmentId, cancellationToken);
                }

                // Retourner le segment mis à jour
                var segmentDto = new TransportSegmentDto(updateSegment);

                return new ResponseHttp
                {
                    Resultat = segmentDto,
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

        /// <summary>
        /// Met à jour le statut du shipment en fonction de la progression des segments
        /// </summary>
        private async Task UpdateShipmentProgress(Guid shipmentId, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentRepository.GetShipmentWithIncludesAsync(shipmentId, new[] { "Segments" }, cancellationToken);
            if (shipment == null) return;

            var segments = shipment.Segments?.ToList() ?? new List<TransportSegment>();
            if (segments.Count == 0) return;

            bool allCompleted = segments.All(s => s.Status == SegmentStatus.Completed);
            bool anyInProgress = segments.Any(s => s.Status == SegmentStatus.InProgress);
            bool anyCancelled = segments.Any(s => s.Status == SegmentStatus.Cancelled);

            if (allCompleted && shipment.Status != ShipmentStatus.Delivered)
            {
                shipment.Status = ShipmentStatus.Delivered;
                await _shipmentRepository.Update(shipment);
                await _shipmentRepository.SaveChange(cancellationToken);
            }
            else if (anyInProgress && shipment.Status != ShipmentStatus.InTransit && shipment.Status != ShipmentStatus.Delivered)
            {
                shipment.Status = ShipmentStatus.InTransit;
                await _shipmentRepository.Update(shipment);
                await _shipmentRepository.SaveChange(cancellationToken);
            }
            else if (anyCancelled && shipment.Status != ShipmentStatus.Cancelled)
            {
                var allCancelledOrPlanned = segments.All(s => s.Status == SegmentStatus.Cancelled || s.Status == SegmentStatus.Planned);
                if (allCancelledOrPlanned)
                {
                    shipment.Status = ShipmentStatus.Cancelled;
                    await _shipmentRepository.Update(shipment);
                    await _shipmentRepository.SaveChange(cancellationToken);
                }
            }
        }
    }
}