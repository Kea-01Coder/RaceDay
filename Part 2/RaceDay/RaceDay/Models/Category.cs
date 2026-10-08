namespace RaceDay.Models
{
    public class Category
    {
        public int CategoryId { get; set; } //Primary Key
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal EntryFee { get; set; }
        public TimeOnly? StartTime { get; set; }
        public decimal DistanceKM { get; set; }
        public int? Capacity { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        //Foreign Key to Event
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}