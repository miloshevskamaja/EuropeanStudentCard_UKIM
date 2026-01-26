using EuropeanStudentCard.Api.Errors;
using EuropeanStudentCard.Clients_iKnow;
using EuropeanStudentCard.Services_IKnow.Eligibility;
using EuropeanStudentCard.Validation_iKnow;
using Microsoft.AspNetCore.Mvc;

namespace EuropeanStudentCard.Controllers_iKnow
{
    [ApiController]
    [Route("mock/iknow")]
    public class IKnowMockController : ControllerBase
    {
        private readonly iKnowClient _client;
        private readonly IKnowStudentValidator _validator;
        private readonly IEscEligibilityService _eligibility;

        public IKnowMockController(
            iKnowClient client,
            IKnowStudentValidator validator,
            IEscEligibilityService eligibility)
        {
            _client = client;
            _validator = validator;
            _eligibility = eligibility;
        }

        // Returns a student by index/ID from the mock iKnow system and validates the payload.
        [HttpGet("studentsbyIndex/{studentId}")]
        public async Task<IActionResult> GetStudentByIndex(string studentId, CancellationToken ct)
        {
            var student = await _client.GetStudentByIndexAsync(studentId, ct);
            if (student is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Code = ApiErrorCodes.StudentNotFound,
                    Message = "Student not found.",
                    Details = new { studentId }
                });
            }

            var validationResult = await _validator.ValidateAsync(student, ct);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Code = ApiErrorCodes.ValidationFailed,
                    Message = "Validation failed for student payload.",
                    Details = validationResult.Errors.Select(e => new
                    {
                        e.PropertyName,
                        e.ErrorMessage
                    })
                });
            }

            return Ok(student);
        }

        // Returns a student only if they are eligible for ESC (regular students: Status = 1).
        [HttpGet("students/{studentId}")]
        public async Task<IActionResult> GetStudentEligibleForEsc(string studentId, CancellationToken ct)
        {
            var student = await _client.GetStudentByIndexAsync(studentId, ct);
            if (student is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Code = ApiErrorCodes.StudentNotFound,
                    Message = "Student not found.",
                    Details = new { studentId }
                });
            }

            var validationResult = await _validator.ValidateAsync(student, ct);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Code = ApiErrorCodes.ValidationFailed,
                    Message = "Validation failed for student payload.",
                    Details = validationResult.Errors.Select(e => new
                    {
                        e.PropertyName,
                        e.ErrorMessage
                    })
                });
            }

            var eligibility = _eligibility.CheckEligibility(student);
            if (!eligibility.IsEligible)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ApiErrorResponse
                {
                    Code = ApiErrorCodes.StudentNotEligible,
                    Message = eligibility.Reason ?? "Student is not eligible for ESC.",
                    Details = new { studentId, status = student.status }
                });
            }

            return Ok(student);
        }

        // Returns all regular students from the mock iKnow system.
        [HttpGet("students")]
        public async Task<IActionResult> GetActiveStudents(CancellationToken ct)
        {
            var students = await _client.GetActiveStudentsAsync(ct);
            return Ok(students);
        }
    }
}
