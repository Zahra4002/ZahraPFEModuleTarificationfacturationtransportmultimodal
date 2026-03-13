using Application.Features.InvoiceFeature.Dtos;

public class InvoiceDTO
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public int Status { get; set; }

    public Guid? ClientId { get; set; }

    public string ClientName { get; set; }

    public Guid? SupplierId { get; set; }

    public string SupplierName { get; set; }

    public Guid? ShipmentId { get; set; }

    public string ShipmentNumber { get; set; }

    public Guid? CurrencyId { get; set; }

    public string CurrencyCode { get; set; }

    public decimal TotalHT { get; set; }

    public decimal TotalVAT { get; set; }

    public decimal TotalTTC { get; set; }

    public List<InvoiceLineDto> Lines { get; set; }
}