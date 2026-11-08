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
    }
}
