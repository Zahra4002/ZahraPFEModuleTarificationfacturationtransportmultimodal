public class InvoiceLineDto
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPriceHT { get; set; }

    public decimal VATRate { get; set; }

    // ✅ ajouter ces champs
    public decimal DiscountPercent { get; set; }

    public Guid? TransportSegmentId { get; set; }

    // totaux
    public decimal TotalHT { get; set; }

    public decimal TotalVAT { get; set; }

    public decimal TotalTTC { get; set; }
}