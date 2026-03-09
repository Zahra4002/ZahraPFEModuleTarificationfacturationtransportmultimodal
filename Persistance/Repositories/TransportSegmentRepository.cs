using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class TransportSegmentRepository : GenericRepository<TransportSegment>, ITransportSegmentRepository
    {
        private readonly CleanArchitecturContext _context;

        public TransportSegmentRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task Delete(TransportSegment segment)
        {
            _context.TransportSegments.Remove(segment);
            await Task.CompletedTask;
        }
        public async Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SupplierId, Guid ZoneFromId, Guid ZoneToId)
        {
            bool shipmentFound = await _context.Shipments.AnyAsync(s => s.Id == ShipmentId);
            if (!shipmentFound)
                return new Dictionary<bool, string> { { false, $"Shipment with ID: {ShipmentId} not found." } };

            bool supplierFound = await _context.Suppliers.AnyAsync(s => s.Id == SupplierId);
            if (!supplierFound)
                return new Dictionary<bool, string> { { false, $"Supplier with ID: {SupplierId} not found." } };

            bool zoneFromFound = await _context.Zones.AnyAsync(z => z.Id == ZoneFromId);
            if (!zoneFromFound)
                return new Dictionary<bool, string> { { false, $"Zone with ID: {ZoneFromId} not found." } };

            bool zoneToFound = await _context.Zones.AnyAsync(z => z.Id == ZoneToId);
            if (!zoneToFound)
                return new Dictionary<bool, string> { { false, $"Zone with ID: {ZoneToId} not found." } };

            // Check if the TransportSegment exists
            bool segmentExists = await _context.TransportSegments.AnyAsync(ts =>
                ts.ShipmentId == ShipmentId &&
                ts.SupplierId == SupplierId &&
                ts.ZoneFromId == ZoneFromId &&
                ts.ZoneToId == ZoneToId);

            if (segmentExists)
                return new Dictionary<bool, string> { { true, "Transport Segment already exists." } };

            return new Dictionary<bool, string> { { false, "Transport Segment does not exist and can be created." } };
        }

        public async Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SegmentId)
        {
            bool shipmentFound = await _context.Shipments.AnyAsync(s => s.Id == ShipmentId);
            if (!shipmentFound)
                return new Dictionary<bool, string> { { false, $"Shipment with ID: {ShipmentId} not found." } };
            bool segmentFound = await _context.TransportSegments.AnyAsync(s => s.Id == SegmentId);
            if (!segmentFound)
                return new Dictionary<bool, string> { { false, $"Transport Segment with ID: {SegmentId} not found." } };

            return new Dictionary<bool, string> { { true, "Transport Segment already exists." } };

        }

    }
}