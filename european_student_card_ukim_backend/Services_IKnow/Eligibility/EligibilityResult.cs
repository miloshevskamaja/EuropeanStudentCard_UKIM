namespace EuropeanStudentCard.Services_IKnow.Eligibility
{
    public sealed record EligibilityResult(
        bool IsEligible,
        string? Reason = null
    );
}
