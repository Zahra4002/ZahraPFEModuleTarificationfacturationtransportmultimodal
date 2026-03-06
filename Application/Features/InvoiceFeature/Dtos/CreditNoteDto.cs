using System;
using System.Collections.Generic;

namespace Application.Features.InvoiceFeature.Dtos
{
    public class CreditNoteDto
    {
        public Guid Id { get; set; }
        public string CreditNoteNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        // Informations de la facture originale
        public string? ClientName { get; set; }
        public string? CurrencyCode { get; set; }

        // Lignes de l'avoir
        public List<CreditNoteLineDto> Lines { get; set; } = new();
    }

    public class CreditNoteLineDto
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceHT { get; set; }
        public decimal VATRate { get; set; }
        public decimal DiscountPercent { get; set; }

        // Propriétés calculées
        public decimal TotalHT => Quantity * UnitPriceHT;
        public decimal TotalVAT => TotalHT * (VATRate / 100);
        public decimal TotalTTC => TotalHT + TotalVAT;
    }
}