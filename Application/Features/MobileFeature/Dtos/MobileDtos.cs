// Application/Features/MobileFeature/Dtos/MobileDtos.cs
namespace Application.Features.MobileFeature.Dtos
{
    public class ShipmentTrackingDto
    {
        public Guid Id { get; set; }
        public string ShipmentNumber { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public List<SegmentTrackingDto> Segments { get; set; }
    }

    public class SegmentTrackingDto
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public string TransportMode { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string Status { get; set; }
    }

    public class ClientQuoteDto
    {
        public Guid Id { get; set; }
        public string QuoteNumber { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsExpired { get; set; }
        public string OriginAddress { get; set; }
        public string DestinationAddress { get; set; }
    }

    public class ClientInvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public string Status { get; set; }
        public bool IsOverdue { get; set; }
    }
}