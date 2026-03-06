using Domain.Entities;
using Domain.Enums;

namespace Application.Features.InvoiceFeature.Dtos
{
    public class InvoiceDTO
    {
        // Propriétés de base
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }

        // Relations
        public Guid? ClientId { get; set; }
        public virtual Client? Client { get; set; }
        public Guid? SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public Guid? ShipmentId { get; set; }
        public virtual Shipment? Shipment { get; set; }

        // Montants
        public decimal TotalHT { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalTTC { get; set; }
        public decimal AmountPaid { get; set; }

        // ✅ AJOUTER CES PROPRIÉTÉS MANQUANTES
        public decimal Balance => TotalTTC - AmountPaid;
        public bool IsOverdue => Status == InvoiceStatus.Envoyee && DueDate < DateTime.UtcNow;
        public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate).Days : 0;

        // ✅ AJOUTER CES PROPRIÉTÉS DÉNORMALISÉES
        public string? ClientName { get; set; }
        public string? SupplierName { get; set; }
        public string? ShipmentNumber { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Devise
        public Guid? CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }
        public decimal ExchangeRate { get; set; }

        // Notes
        public string? Notes { get; set; }

        // Collections
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }
}