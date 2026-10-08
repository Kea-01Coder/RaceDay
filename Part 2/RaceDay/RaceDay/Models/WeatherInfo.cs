namespace RaceDay.Models
{
    public class WeatherInfo
    {
        public int WeatherId { get; set; } //Primary Key
        public DateOnly ForecastDate { get; set; }
        public decimal? Temperature { get; set; }
        public string? Conditions { get; set; }
        public decimal? WindSpeed { get; set; }
        public decimal? Humidity { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Foreign Key to Event
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
