using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransportSegmentRepository : IGenericRepository<TransportSegment>
    {
        Task Delete(TransportSegment segment);
        Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SupplierId, Guid ZoneFromId, Guid ZoneToId);
        Task<Dictionary<bool, string>> IsExitAsync(Guid ShipmentId, Guid SegmentId);

    }
}