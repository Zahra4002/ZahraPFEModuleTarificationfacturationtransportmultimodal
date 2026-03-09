using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PriceCalculationFeature.Dtos
{
    public class PriceCalculationResult
    {
        public decimal BaseCost { get; set; }
        public decimal SurchargesTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalCost { get; set; }
        public string CurrencyCode { get; set; } = "EUR";
        public PriceBreakdown Breakdown { get; set; } = new();
        public string? AppliedContractNumber { get; set; }
        public string? AppliedTariffGridCode { get; set; }
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PriceBreakdown
    {
        public List<PriceComponent> BaseComponents { get; set; } = new();
        public List<PriceComponent> Surcharges { get; set; } = new();
        public List<PriceComponent> Taxes { get; set; } = new();
        public List<PriceComponent> Discounts { get; set; } = new();
    }

    public class PriceComponent
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "EUR";
        public string? CalculationBasis { get; set; }
        public string? Reference { get; set; }
    }
}
