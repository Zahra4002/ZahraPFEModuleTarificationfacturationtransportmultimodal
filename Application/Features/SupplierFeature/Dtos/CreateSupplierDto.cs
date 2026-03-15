using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Dtos
{
    public class CreateSupplierDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; } = string.Empty;
        public string DefaultCurrencyCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;


    }


}
