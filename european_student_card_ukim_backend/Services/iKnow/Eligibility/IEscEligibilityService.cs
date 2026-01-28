using EuropeanStudentCard.DTO.iKnow;

namespace EuropeanStudentCard.Services.iKnow.Eligibility
{
    public interface IEscEligibilityService
    {
        EligibilityResult CheckEligibility(IKnowStudentDto student);
    }
}
