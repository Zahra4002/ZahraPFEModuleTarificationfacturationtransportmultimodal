using System;

namespace Application.Features.DashboardFeature.Dtos
{
    public class MarginStatsDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalMargin { get; set; }
        public decimal AverageMarginPercent { get; set; }
    }
}