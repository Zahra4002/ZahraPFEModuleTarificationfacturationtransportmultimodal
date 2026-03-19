using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PriceCalculationFeature.Dtos
{
    public class BaseCompoment
    {
        public string Code { get; set; }
        public string description { get; set; }
        public decimal amout { get; set; }
        public string currencyCode { get; set; }
        public string calculationbasis { get; set; }
        public string reference { get; set; }

    }
}
