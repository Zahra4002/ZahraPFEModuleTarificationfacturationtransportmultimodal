using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Quote : Entity
    {
        public string QuoteNumber { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }

        // Copie des données d'expédition potentielle
        public Address OriginAddress { get; set; } = new();
        public Address DestinationAddress { get; set; } = new();
        public decimal? WeightKg { get; set; }
        public decimal? VolumeM3 { get; set; }
        public Guid? MerchandiseTypeId { get; set; }

        // Tarification
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public string CurrencyCode { get; set; } = "EUR";

        // Validité
        public DateTime ValidUntil { get; set; }
        public bool IsAccepted { get; set; } = false;
        public DateTime? AcceptedAt { get; set; }

        // Détail du calcul
        public string? PriceBreakdownJson { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual MerchandiseType? MerchandiseType { get; set; }
        public virtual Shipment? Shipment { get; set; }

        public bool IsValid => !IsAccepted && DateTime.UtcNow <= ValidUntil;
    }
}
