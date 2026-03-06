using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShipmentRepository
    {
        Task<Shipment?> GetByIdWithDetailsAsync(Guid id);
        Task<Shipment?> GetByQuoteIdAsync(Guid quoteId);
        Task AddAsync(Shipment shipment);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<List<Shipment>> GetShipmentsByDateRangeWithSegmentsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}
