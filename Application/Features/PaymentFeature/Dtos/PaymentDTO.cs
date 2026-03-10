using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.PaymentFeature.Dtos
{
    internal class PaymentDTO
    {
        public Guid? InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
