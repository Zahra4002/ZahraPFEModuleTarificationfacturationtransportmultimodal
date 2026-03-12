using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.TestFeature.Dtos
{
    public class ClientDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Address BullingAddress { get; set; }
        public Address? ShippingAddress { get; set; }
        public string DefaultCurrencyCode { get; set; }
        public decimal CreditLimit { get; set; }
        public int PaymentTermDays { get; set; }
        public bool IsActive { get; set; }

        public ClientDTO() { }
    }
}
