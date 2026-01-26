using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EuropeanStudentCard.Models
{
    public class StudentCard
    {
        [Key, ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        public string CardNumber { get; set; }
        public string? DisplayName {get; set;}

        public string PersonIdentifier { get; set; }

        public string IssuerIdentifier { get; set; }
        
        public string CardStatusType { get; set; } // "ACTIVE", "INACTIVE"
        
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        
        public DateTime? ExpiresAt { get; set; } = DateTime.Now + TimeSpan.FromDays(360);

        public string CardType { get; set; } = "STUDENT";

        // Navigation property to Student
        public Student Student { get; set; }
    }
}
