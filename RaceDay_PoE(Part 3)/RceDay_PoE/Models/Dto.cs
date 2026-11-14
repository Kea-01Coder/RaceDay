namespace RaceDay_PoE.Models
{
    public class Dto
    {
        // --- AUTH DTOS ---
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public UserDto User { get; set; }
        }

        public class UserDto
        {
            public int UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
        }

        // --- EVENT DTOS ---
        public class EventDto
        {
            public int EventId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime EventDateTime { get; set; }
            public string Location { get; set; }
            public decimal EntryFee { get; set; }
            public string Status { get; set; }
        }

        public class EventListResponse
        {
            public List<EventDto> Events { get; set; }
            public int TotalCount { get; set; }
        }

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

        public class EnrollmentDto
        {
            public int EnrollmentId { get; set; }
            public string EventTitle { get; set; }
            public DateTime EventDate { get; set; }
            public string CategoryName { get; set; }
            public string Status { get; set; }
            public string PaymentStatus { get; set; }
        }

        public class SubmitResultRequest
        {
            public int EnrollmentId { get; set; }
            public TimeOnly FinishTime { get; set; }
            public int Position { get; set; }
            public int OverallRank { get; set; }
            public int CategoryRank { get; set; }
        }

        public class ResultDto
        {
            public int ResultId { get; set; }
            public string EventTitle { get; set; }
            public TimeOnly FinishTime { get; set; }
            public int Position { get; set; }
            public int OverallRank { get; set; }
            public int CategoryRank { get; set; }
            public bool Certified { get; set; }
        }
    }
}

