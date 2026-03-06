using System;

namespace Application.Features.InvoiceFeature.Dtos
{
    public class AddInvoiceLineDto
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPriceHT { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal VatRate { get; set; }
        public Guid? TransportSegmentId { get; set; }
        public Guid? SurchargeId { get; set; }
    }
}