namespace RaceDay_PoE.Models
{
    public class AuthDtos
    {
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
    }
}