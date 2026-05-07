using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.PaymentFeature.Dtos
{
    public  class PaymentDTO
    {
        public Guid? Id { get; set; }
        public Guid? InvoiceId { get; set; }
        
        // ✅ AJOUTER: Informations de la facture
        public string? InvoiceNumber { get; set; }
        public string? ClientName { get; set; }
        
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        
        // ✅ AJOUTER: Date de création
        public DateTime? CreatedAt { get; set; }
    }
}
