using Domain.Common;

namespace Domain.Entities
{
    /// <summary>
    /// Entité représentant un taux de change
    /// </summary>
    public class ExchangeRate : Entity
    {
        public Guid FromCurrencyId { get; set; }
        public Guid ToCurrencyId { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Source { get; set; } // Ex: "BCE", "Manual"

        // Navigation properties
        public virtual Currency FromCurrency { get; set; } = null!;
        public virtual Currency ToCurrency { get; set; } = null!;

        public bool IsValidAt(DateTime date) =>
            date >= EffectiveDate && (!ExpiryDate.HasValue || date <= ExpiryDate.Value);

        public decimal Convert(decimal amount) => amount * Rate;
        public decimal ConvertReverse(decimal amount) => Rate != 0 ? amount / Rate : 0;
    }
}
