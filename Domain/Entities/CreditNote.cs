using Domain.Common;

namespace Domain.Entities
{
    public class CreditNote : Entity  // ← Hérite de Entity (qui a Id)
    {
        public string CreditNoteNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public virtual ICollection<InvoiceLine> Lines { get; set; }
    }
}