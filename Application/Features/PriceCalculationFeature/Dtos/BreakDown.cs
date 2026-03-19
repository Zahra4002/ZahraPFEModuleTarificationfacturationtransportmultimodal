using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.PriceCalculationFeature.Dtos
{
    public class BreakDown
    {
        public List<BaseCompoment> baseCompoments { get; set; } = new List<BaseCompoment>();
        public List<BaseCompoment> Surcharges { get; set; } = new List<BaseCompoment>();

        public List<BaseCompoment> Taxes { get; set; } = new List<BaseCompoment>();
    }
}
