using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PriceCalculationFeature.Dtos
{
    public class ResultDto
    {
        public decimal baseCost { get; set; }
        public decimal surchargesTotal { get; set; }
        public decimal subtotal { get; set; }
        public decimal taxTotal { get; set; }
        public string currencyCode { get; set; }
        public BreakDown breakDown { get; set; }
        public string appliedContractNumber { get; set; }
        public string appliedTariffGrid { get; set; }
        public DateTime? calculatedAtDate { get; set; }
    }
}
