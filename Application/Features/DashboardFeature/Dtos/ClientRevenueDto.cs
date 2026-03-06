using System;

namespace Application.Features.DashboardFeature.Dtos
{
    public class ClientRevenueDto
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }
}