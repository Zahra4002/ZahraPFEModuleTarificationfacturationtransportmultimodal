using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Supplier : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public string? TaxId { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string Address { get; set; }

        public string DefaultCurrencyCode { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Contract> Contracts { get; set; }

        public virtual ICollection<TransportSegment> TransportSegments { get; set; }

    }
}
