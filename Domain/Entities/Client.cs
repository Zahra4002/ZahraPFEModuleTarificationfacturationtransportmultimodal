using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Client : Entity
    {
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

        public virtual ICollection<Contract> Contracts { get; set; }

        public virtual ICollection <Invoice> invoices { get; set; }

        public virtual ICollection<Shipment> shipments { get; set; }

        public virtual ICollection<Quote> Quotes { get; set; }



    }
}
