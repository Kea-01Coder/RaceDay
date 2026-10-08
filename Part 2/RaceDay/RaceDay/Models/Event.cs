using System.Net.NetworkInformation;

namespace RaceDay.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int MaxParticipants { get; set; }
        public decimal EntryFee { get; set; }
        public decimal? EarlyBirdFee { get; set; }
        public DateTime? EarlyBirdDate { get; set; }
        public string Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
