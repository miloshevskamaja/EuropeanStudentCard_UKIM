namespace EuropeanStudentCard.Models.Esc
{
    public class EscIssueRequestDto
    {

        // these variables depend on the ESC specification
        public string ExternalStudentId { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Email { get; set; }
        public string? StudyProgram { get; set; }

    }
}
