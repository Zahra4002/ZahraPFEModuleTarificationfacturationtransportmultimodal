using System;
using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Dtos
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; } = string.Empty;
        public string DefaultCurrencyCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // ✅ COLLECTIONS
        public List<ContractSummaryDto> Contracts { get; set; } = new ();
        public List<TransportSegmentDto> TransportSegments { get; set; } = new();
        public List<InvoiceSummaryDTO> Invoices { get; set; } = new();
    }
}