namespace Application.Features.DashboardFeature.Dtos
{
    public class InvoiceStatusStatsDto
    {
        public int TotalInvoices { get; set; }
        public int DraftCount { get; set; }
        public int EmittedCount { get; set; }
        public int PaidCount { get; set; }
        public int OverdueCount { get; set; }
        public int DisputedCount { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal TotalOverdue { get; set; }
    }
}