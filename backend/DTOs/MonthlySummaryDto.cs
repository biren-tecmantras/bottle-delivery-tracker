namespace BottleDeliveryTracker.Backend.DTOs
{
    public sealed class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }

        public string MonthLabel => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    }
}
