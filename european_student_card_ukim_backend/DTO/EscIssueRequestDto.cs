namespace EuropeanStudentCard.Models.Esc
{
    public class EscIssueRequestDto
    {
        // Internal DTO representing ESC-related student data decoupled from the ESC Router API specification.

        // these variables may be changed 
        public string ExternalStudentId { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Email { get; set; }
        public string? StudyProgram { get; set; }
    }
}
