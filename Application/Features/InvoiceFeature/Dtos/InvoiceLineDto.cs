namespace Application.Features.InvoiceFeature.Dtos
{
    public class InvoiceLineDto
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPriceHT { get; set; }

        public decimal VATRate { get; set; }

        public decimal TotalHT { get; set; }

        public decimal TotalVAT { get; set; }

        public decimal TotalTTC { get; set; }

        public Guid? TransportSegmentId { get; set; }

        public decimal DiscountPercent { get; set; } = 0;
    }
}