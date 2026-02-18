using Domain.Common;
using Intuit.Ipp.Data;

namespace Domain.Entities
{
    public class Currency: Entity
    {
        public string Code { get; set; } = string.Empty; // ISO 4217 (EUR, USD, TND)
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public int DecimalPlaces { get; set; } = 2;
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

        // Navigation properties
        public virtual ICollection<ExchangeRate> ExchangeRatesFrom { get; set; } = new List<ExchangeRate>();
        public virtual ICollection<ExchangeRate> ExchangeRatesTo { get; set; } = new List<ExchangeRate>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}