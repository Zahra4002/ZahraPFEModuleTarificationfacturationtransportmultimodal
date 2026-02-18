using System;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Payment:Entity
    {
        public Guid? InvoiceId { get; set; }
        /// <summary>
        /// Gets or sets the invoice associated with the current entity.
        /// </summary>
        public virtual Invoice Invoice { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }

        
    }
}
