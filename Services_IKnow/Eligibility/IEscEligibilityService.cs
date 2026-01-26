using EuropeanStudentCard.Models_iKnow;

namespace EuropeanStudentCard.Services_IKnow.Eligibility
{
    public interface IEscEligibilityService
    {
        EligibilityResult CheckEligibility(IKnowStudentDto student);
    }
}
