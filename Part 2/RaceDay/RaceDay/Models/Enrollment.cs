namespace RaceDay.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; } //Primary Key

        //Registration status
        public string? Status { get; set; }
        public DateTime? EntryDate { get; set; }

        //Payment fields
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentRef { get; set; }
        public decimal? AmountPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? InvoiceNumber { get; set; }

        //Participant details
        public string ParticipantName { get; set; }
        public int? ParticipantUserId { get; set; }
        public string ParticipantIdNumber { get; set; }
        public DateOnly? ParticipantDob { get; set; }
        public string? Gender { get; set; }

        //Emergency contact
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? MedicalConditions { get; set; }

        //Race day details
        public string? BibNumber { get; set; }
        public DateTime? WaveStart { get; set; }
        public string? AgeCategory { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Foreign keys
        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
    }
}
