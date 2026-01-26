using EuropeanStudentCard.Data;
using EuropeanStudentCard.DTO;
using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EuropeanStudentCard.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly AppDbContext _context;
    private readonly ILogger<CardController> _logger;

    public CardController(
        ICardService cardService, 
        AppDbContext context,
        ILogger<CardController> logger)
    {
        _cardService = cardService ?? throw new ArgumentNullException(nameof(cardService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Helper Endpoints for Testing
    // TODO: remove when university system integration is complete
    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _context.Students.ToListAsync();
        return Ok(students);
    }

    [HttpPost("students")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var student = new Student
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created test student: {StudentId}", student.Id);
        return CreatedAtAction(nameof(GetStudents), new { id = student.Id }, student);
    }

    // Card Endpoints 

    // Generate a new European Student Card   
    [HttpPost("generate/{studentId}")]
    public async Task<IActionResult> GenerateCard([FromRoute] int studentId)
    {
        try
        {
            var card = await _cardService.GenerateCardAsync(studentId);
            _logger.LogInformation("Generated card {CardNumber} for student {StudentId}", card.CardNumber, studentId);
            return CreatedAtAction(nameof(GetCard), new { cardNumber = card.CardNumber }, card);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to generate card for student {StudentId}", studentId);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating card for student {StudentId}", studentId);
            return BadRequest(new { error = "Failed to generate card", details = ex.Message });
        }
    }

    // Get all cards from the local database.
    [HttpGet]
    public async Task<IActionResult> GetAllCards()
    {
        var cards = await _cardService.GetAllCardsAsync();
        return Ok(cards);
    }
    
    // Get card details by card number (ESCN)
    [HttpGet("{cardNumber}")]
    public async Task<IActionResult> GetCard(string cardNumber)
    {
        var card = await _cardService.GetCardByNumberAsync(cardNumber);
        if (card == null)
        {
            return NotFound(new { error = "Card not found", cardNumber });
        }

        return Ok(card);
    }
    
    // Get card status from ESC Router
    [HttpGet("{cardNumber}/status")]
    public async Task<IActionResult> GetCardStatus(string cardNumber)
    {
        try
        {
            var status = await _cardService.GetCardStatusFromRouterAsync(cardNumber);
            if (status == null)
            {
                return NotFound(new { error = "Card not found in ESC Router", cardNumber });
            }

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching card status for {CardNumber}", cardNumber);
            return StatusCode(StatusCodes.Status502BadGateway, 
                new { error = "Failed to fetch card status from ESC Router", details = ex.Message });
        }
    }
    
    // Get QR code image for a card.
    [HttpGet("{cardNumber}/qr")]
    public async Task<IActionResult> GetQrCode(string cardNumber)
    {
        try
        {
            var qrCode = await _cardService.GetQrCodeAsync(cardNumber);
            if (qrCode == null)
            {
                return NotFound(new { error = "QR code not found for card", cardNumber });
            }

            // ESC Router returns SVG format
            return File(qrCode, "image/svg+xml", $"card-{cardNumber}-qr.svg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching QR code for {CardNumber}", cardNumber);
            return StatusCode(StatusCodes.Status502BadGateway, 
                new { error = "Failed to fetch QR code from ESC Router", details = ex.Message });
        }
    }
    
    // Update card information
    [HttpPut("{cardNumber}")]
    public async Task<IActionResult> UpdateCard(string cardNumber, [FromBody] UpdateCardDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var card = await _cardService.UpdateCardAsync(
                cardNumber, 
                dto.CardStatusType, 
                dto.ExpiresAt);

            if (card == null)
            {
                return NotFound(new { error = "Card not found", cardNumber });
            }

            _logger.LogInformation("Updated card {CardNumber}", cardNumber);
            return Ok(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {CardNumber}", cardNumber);
            return BadRequest(new { error = "Failed to update card", details = ex.Message });
        }
    }
    
    // Revoke/delete a card.    
    [HttpDelete("{cardNumber}")]
    public async Task<IActionResult> RevokeCard(string cardNumber)
    {
        try
        {
            var success = await _cardService.RevokeCardAsync(cardNumber);
            if (!success)
            {
                return NotFound(new { error = "Card not found", cardNumber });
            }

            _logger.LogInformation("Revoked card {CardNumber}", cardNumber);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking card {CardNumber}", cardNumber);
            return BadRequest(new { error = "Failed to revoke card", details = ex.Message });
        }
    }
}
