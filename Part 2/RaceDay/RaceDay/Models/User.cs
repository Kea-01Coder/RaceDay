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

        public string? Phone { get; set; } //Nullable

        public string? IDNumber { get; set; }  //Nullable

        public DateTime? DateOfBirth { get; set; }   //Nullable

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Event> Events { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
