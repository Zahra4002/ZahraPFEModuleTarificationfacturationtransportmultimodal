namespace Application.Features.DashboardFeature.Dtos
{
    public class RevenueByModeDto
    {
        public string TransportMode { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
        public int ShipmentCount { get; set; }
    }
}