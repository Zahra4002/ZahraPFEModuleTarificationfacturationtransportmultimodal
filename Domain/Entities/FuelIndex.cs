using Domain.Common;

namespace Domain.Entities
{
    public class FuelIndex : Entity
    {
        public DateTime Date { get; set; }
        public decimal IndexValue { get; set; }
        public decimal BasePrice { get; set; } // Prix de référence
        public string? Source { get; set; } // Source de l'index (ex: "PLATTS")
        public string? Notes { get; set; }
    }
}
