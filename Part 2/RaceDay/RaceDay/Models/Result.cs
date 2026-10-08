namespace RaceDay.Models
{
    public class Result
    {
        public int ResultId { get; set; } //Primary Key 
        public TimeOnly FinishTime { get; set; }
        public int Position { get; set; }
        public int OverallRank { get; set; }
        public int CategoryRank { get; set; }
        public bool? Certified { get; set; } = false;
        public string? CertificateUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Foreign Key to Enrollment
        public int EnrollmentId { get; set; } 
        public Enrollment Enrollment { get; set; }
       
    }
}
