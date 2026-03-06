namespace Application.Features.InvoiceFeature.Dtos
{
    public class UpdateInvoiceLineDto
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPriceHT { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal VatRate { get; set; }
    }
}