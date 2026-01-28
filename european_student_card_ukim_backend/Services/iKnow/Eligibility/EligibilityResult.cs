namespace EuropeanStudentCard.Services.iKnow.Eligibility
{
    public sealed record EligibilityResult(
        bool IsEligible,
        string? Reason = null
    );
}
