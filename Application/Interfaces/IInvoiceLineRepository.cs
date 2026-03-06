using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInvoiceLineRepository : IGenericRepository<InvoiceLine>
    {
        Task<List<InvoiceLine>> GetByInvoiceIdAsync(Guid invoiceId);
        Task DeleteByInvoiceIdAsync(Guid invoiceId);
        Task<InvoiceLine?> GetByIdWithDetailsAsync(Guid id);           // ✅ Ajouté
        Task AddAsync(InvoiceLine line);                               // ✅ Ajouté
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<InvoiceLine?> GetByIdAsync(Guid id);
        Task UpdateAsync(InvoiceLine line);
        Task<Invoice?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteAsync(InvoiceLine line);
    }
}