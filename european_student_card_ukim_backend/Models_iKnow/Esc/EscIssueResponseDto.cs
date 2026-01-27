namespace EuropeanStudentCard.Models_iKnow.Esc
{
    public class EscIssueResponseDto
    {
        public string Escn { get; set; } = default!;
        public string Status { get; set; } = "ACTIVE";
        public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
