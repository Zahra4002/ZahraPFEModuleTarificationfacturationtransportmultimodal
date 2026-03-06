using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Invoice : Entity
    {
        public string InvoiceNumber { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool IsSupplierInvoice { get; set; }
        public virtual Client? Client { get; set; }
        public virtual String? ClientAddress { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? ShipmentId { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public string? SupplierName { get; set; }
        public virtual Shipment? Shipment { get; set; }
        public string? ShipmentNumber { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalTTC { get; set; }
        public decimal AmountPaid { get; set; }
        public Guid? CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string? Notes { get; set; }
        public decimal Balance => TotalTTC - AmountPaid;
        public bool IsOverdue => Status == InvoiceStatus.Envoyee && DueDate < DateTime.UtcNow;
        public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate).Days : 0;
        public virtual ICollection<CreditNote> CreditNotes { get; set; } = new List<CreditNote>();
        public virtual ICollection<Payment> Payements { get; set; }

        public virtual ICollection<InvoiceLine> Lines { get; set; }
        public string? CurrencyCode { get; set; }
    }
}
