using EuropeanStudentCard.Data;
using EuropeanStudentCard.DTO;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Models;
using EuropeanStudentCard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EuropeanStudentCard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly AppDbContext _context; // Injecting context just to seed data easily

        public CardController(ICardService cardService, AppDbContext context)
        {
            _cardService = cardService;
            _context = context;
        }
        // these 2 http methods should be in StudentController.cs (CREATE NEW CONTROLLER FOR THEM)
        [HttpGet("student")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        // Helper endpoint to create dummy students so we can test
        [HttpPost("seed-student")]
        public async Task<IActionResult> SeedStudent([FromBody] CreateStudentDto student)
        {
            var studentEntity = new Student
            {
                Name = student.Name,
                Email = student.Email,
            };
            
            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();
            return Ok(student);
        }

        [HttpPost("generate/{studentId}")]
        public async Task<IActionResult> GenerateCard([FromRoute] int studentId)
        {
            try
            {
                var card = await _cardService.GenerateCardAsync(studentId);
                return Ok(card);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{cardNumber}/status")]
        public async Task<IActionResult> GetStatus(string cardNumber)
        {
            var card = await _cardService.GetCardStatusAsync(cardNumber);
            if (card == null)
            {
                return NotFound("Card not found");
            }
            return Ok(new 
            { 
                CardNumber = card.CardNumber, 
                CardStatusType = card.CardStatusType,
                PersonIdentifier = card.PersonIdentifier,
                StudentName = card.Student?.Name,
                IssuedAt = card.IssuedAt,
                ExpiresAt = card.ExpiresAt
            });
        }
    }
}
