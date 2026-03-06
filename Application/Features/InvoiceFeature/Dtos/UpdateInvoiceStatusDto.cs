using Domain.Enums;

namespace Application.Features.InvoiceFeature.Dtos
{
    public class UpdateInvoiceStatusDto
    {
        public InvoiceStatus Status { get; set; }
        
    }
}