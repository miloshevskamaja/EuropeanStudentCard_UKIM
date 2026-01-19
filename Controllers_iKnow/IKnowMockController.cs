using EuropeanStudentCard.Clients_iKnow;
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

        public IKnowMockController(iKnowClient client, IKnowStudentValidator validator)
        {
            _client = client;
            _validator = validator;
        }

        [HttpGet("studentsbyIndex/{studentId}")]
        public async Task<IActionResult> GetStudent(string studentId, CancellationToken ct)
        {
            var student = await _client.GetStudentByIndexAsync(studentId, ct);
            if (student is null) return NotFound();

            var result = await _validator.ValidateAsync(student, ct);
            if (!result.IsValid)
                return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

            return Ok(student);
        }

        [HttpGet("students/{studentId}")]
        public async Task<IActionResult> GetStudentbyStatus(string studentId, CancellationToken ct)
        {
            var student = await _client.GetStudentByIndexAsync(studentId, ct);
            if (student is null)
                return NotFound(new { message = "Student not found." });

            var validationResult = await _validator.ValidateAsync(student, ct);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }));

          
            if (student.status != 1)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "ESC card is valid only for active/regular students."
                });
            }

            return Ok(student);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetActiveStudents(CancellationToken ct)
        {
            var students = await _client.GetActiveStudentsAsync(ct);
            return Ok(students);
        }
    }
}
