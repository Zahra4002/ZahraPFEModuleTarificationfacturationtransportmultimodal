using AutoMapper;
using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<PagedList<Invoice>> GetAllWithDetailsAsync(
            int? pageNumber,
            int? pageSize,
            CancellationToken cancellationToken);

        Task<Invoice?> GetByIdWithDetailsAsync(
            Guid id,
            CancellationToken cancellationToken);
        Task<List<Invoice>> GetOverdueInvoicesAsync();
        Task<Invoice?> GetByShipmentIdAsync(Guid shipmentId);
        Task<Invoice?> GetLastInvoiceAsync();
        Task AddAsync(Invoice invoice);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Invoice?> GetByIdAsync(Guid id);
        Task UpdateAsync(Invoice invoice);
        Task<Invoice?> GetByIdWithDetailsAsync(Guid id);
        Task<Invoice?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetInvoicesWithClientsByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetInvoicesByDateRangeWithShipmentsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetOverdueInvoicesWithDetailsAsync(DateTime currentDate, CancellationToken cancellationToken = default);




    }
}