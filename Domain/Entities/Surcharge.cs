using Domain.Common;
using Domain.Enums;
using System.Data;


namespace Domain.Entities
{
    public class Surcharge: Entity
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public SurchargeType Type { get; set; }
        public CalculationType CalculationType { get; set; }
        public decimal Value { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<SurchargeRule> Rules { get; set; }

    }
}
