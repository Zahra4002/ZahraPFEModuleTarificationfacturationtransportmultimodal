using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Dtos
{
    public class UpdateSupplierDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DefaultCurrencyCode { get; set; }
        public bool? IsActive { get; set; }
    }
    }
        