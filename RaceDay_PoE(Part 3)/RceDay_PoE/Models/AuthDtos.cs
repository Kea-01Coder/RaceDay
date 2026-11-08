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
    }
}