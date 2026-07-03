namespace BottleDeliveryTracker.Backend.Models
{
    public class DeliveryRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
