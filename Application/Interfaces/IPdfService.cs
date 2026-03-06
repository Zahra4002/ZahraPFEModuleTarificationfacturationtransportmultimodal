using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice);
    }
}