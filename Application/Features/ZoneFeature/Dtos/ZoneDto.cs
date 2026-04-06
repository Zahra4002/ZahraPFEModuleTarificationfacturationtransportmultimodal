using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Dtos
{
    public class ZoneDto
    {
        public  Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string? Region { get; set; }

        public string? Description { get; set; }

        public decimal? TaxRate { get; set; }

        public bool IsActive { get; set; } = true;

        public ZoneDto(Zone zone)
        {
            this.Id = zone.Id;
            this.Code = zone.Code;
            this.Name = zone.Name;
            this.Country = zone.Country;
            this.Region = zone.Region;
            this.Description = zone.Description;
            this.TaxRate = zone.TaxRate;
            this.IsActive = zone.IsActive;
        }

    }

    
}
