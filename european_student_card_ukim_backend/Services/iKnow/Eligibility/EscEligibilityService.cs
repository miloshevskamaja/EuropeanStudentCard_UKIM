using EuropeanStudentCard.DTO.iKnow;

namespace EuropeanStudentCard.Services.iKnow.Eligibility
{
    public class EscEligibilityService : IEscEligibilityService
    {
        public EligibilityResult CheckEligibility(IKnowStudentDto student)
        {
            // 1 = regular, 0 = not regular
            if (student.status != 1)
                return new EligibilityResult(false, "ESC card is valid only for regular students (status=1).");

            return new EligibilityResult(true);
        }
    }
}
