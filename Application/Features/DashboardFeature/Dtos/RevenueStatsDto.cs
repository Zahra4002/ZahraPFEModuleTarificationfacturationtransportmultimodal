using System;
using System.Collections.Generic;

namespace Application.Features.DashboardFeature.Dtos
{
    public class RevenueStatsDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalMargin { get; set; }
        public decimal MarginPercent { get; set; }
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
    }

    public class MonthlyBreakdownDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Margin { get; set; }
    }
}