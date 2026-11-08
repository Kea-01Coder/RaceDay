using static RaceDay_PoE.Models.AuthDtos;

namespace RaceDay_PoE.Models
{
    public class EventDtos
    {
        public class EventDetailsResponse
        {
            public EventDto Event { get; set; }
            public List<CategoryDto> Categories { get; set; }
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public decimal EntryFee { get; set; }
            public decimal DistanceKM { get; set; }
        }

        public class CreateEventRequest
        {
            public string Title { get; set; }
            public string? Description { get; set; }
            public DateTime EventDateTime { get; set; }
            public string Location { get; set; }
            public int MaxParticipants { get; set; }
            public decimal EntryFee { get; set; }
            public decimal? EarlyBirdFee { get; set; }
            public DateTime? EarlyBirdDate { get; set; }
        }
    }
}
