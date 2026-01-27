using EuropeanStudentCard.Models_iKnow.Esc;
using EuropeanStudentCard.Models_iKnow;

namespace EuropeanStudentCard.Mapping
{
    public static class IKnowToEscMapper
    {
        public static EscIssueRequestDto ToEscIssueRequest(IKnowStudentDto s)
        {
            string Clean(string? x) => string.IsNullOrWhiteSpace(x) ? string.Empty : x.Trim();

            return new EscIssueRequestDto
            {
                ExternalStudentId = Clean(s.index),
                FirstName = Clean(s.name),
                LastName = Clean(s.surname),
                Email = string.IsNullOrWhiteSpace(s.email) ? null : s.email.Trim(),
                StudyProgram = string.IsNullOrWhiteSpace(s.programmeName) ? null : s.programmeName.Trim()
            };
        }
    }
}
