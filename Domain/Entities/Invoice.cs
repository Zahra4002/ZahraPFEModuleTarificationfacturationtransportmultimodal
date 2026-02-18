using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Invoice : Entity
    {
        public string InvoiceNumber { get; set; }
        public Guid ClientId { get; set; }

        public virtual Client Client { get; set; }
        public Guid? ShipmentId { get; set; }

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

        public virtual ICollection<Payment> Payements { get; set; }

        public virtual ICollection<InvoiceLine> Lines { get; set; }
        
    }
}
