using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<PagedList<Payment>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<Payment>> GetByInvoiceAsync(string invoiceNumber, CancellationToken ct = default);
        Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

