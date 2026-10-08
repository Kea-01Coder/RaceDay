using Microsoft.VisualBasic;

namespace RaceDay.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public int? Phone { get; set; } //Nullable

        public int? IDNumber { get; set; }  //Nullable

        public int? DateOfBirth { get; set; }   //Nullable

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
