using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface ITransportSegmentRepository : IGenericRepository<TransportSegment>
    {
        Task<List<TransportSegment>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
        Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SupplierId, Guid ZoneFromId, Guid ZoneToId);
        Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SegmentId);
        Task Delete(TransportSegment segment);
        Task<TransportSegment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}