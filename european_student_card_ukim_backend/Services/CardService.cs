using Microsoft.EntityFrameworkCore;
using EuropeanStudentCard.Configuration;
using EuropeanStudentCard.Data;
using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Models;
using Microsoft.Extensions.Options;

namespace EuropeanStudentCard.Services;


public class CardService : ICardService
{
    private readonly AppDbContext _context;
    private readonly IEscRouterClient _escRouterClient;
    private readonly EscRouterSettings _settings;
    private readonly ILogger<CardService> _logger;

    public CardService(
        AppDbContext context,
        IEscRouterClient escRouterClient,
        IOptions<EscRouterSettings> settings,
        ILogger<CardService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _escRouterClient = escRouterClient ?? throw new ArgumentNullException(nameof(escRouterClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<StudentCard> GenerateCardAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var student = await _context.Students.FindAsync(new object[] { studentId }, cancellationToken);
        if (student == null)
        {
            _logger.LogWarning("Student not found: {StudentId}", studentId);
            throw new InvalidOperationException($"Student with ID {studentId} not found");
        }

        // Check if card already exists
        var existingCard = await _context.StudentCards
            .FirstOrDefaultAsync(c => c.StudentId == studentId, cancellationToken);
        if (existingCard != null)
        {
            _logger.LogInformation("Card already exists for student {StudentId}: {CardNumber}", studentId, existingCard.CardNumber);
            return existingCard;
        }

        // 1. Generate Card Number (ESCN) via ESC Router API
        string cardNumber = await _escRouterClient.GenerateEscnAsync(cancellationToken);
        _logger.LogInformation("Generated ESCN: {CardNumber} for student: {StudentId}", cardNumber, studentId);

        // 2. Generate Person Identifier (ESI)
        string personIdentifier = GenerateEsi(studentId);
        var displayName = $"Card of {student.Name}";

        // 3. Create Card Record in local database
        var newCard = new StudentCard
        {
            CardNumber = cardNumber,
            PersonIdentifier = personIdentifier,
            IssuerIdentifier = _settings.Pic,
            CardStatusType = "PENDING",
            DisplayName = displayName,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(4),
            StudentId = studentId
        };

        _context.StudentCards.Add(newCard);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created local card record: {CardNumber}", cardNumber);

        // 4. Register card with ESC Router
        try
        {
            var request = new EscCardRequest
            {
                CardNumber = newCard.CardNumber,
                PersonIdentifier = newCard.PersonIdentifier,
                IssuerIdentifier = newCard.IssuerIdentifier,
                CardStatusType = "ACTIVE",
                DisplayName = newCard.DisplayName,
                IssuedAt = newCard.IssuedAt.ToString("yyyy-MM-dd"),
                ExpiresAt = newCard.ExpiresAt?.ToString("yyyy-MM-dd"),
                CardType = "SMART_CDZ"
            };

            var response = await _escRouterClient.CreateCardAsync(request, cancellationToken);
            
            if (response != null)
            {
                newCard.CardStatusType = "ACTIVE";
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully issued card in ESC Router: {CardNumber}", cardNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to register card with ESC Router. Card saved locally with PENDING status.");
            // Card remains in PENDING status in local database
        }

        return newCard;
    }

    public async Task<List<StudentCard>> GetAllCardsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StudentCards
            .Include(c => c.Student)
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentCard?> GetCardByNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.StudentCards
            .Include(c => c.Student)
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
    }

    public async Task<EscCardStatusResponse?> GetCardStatusFromRouterAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _escRouterClient.GetCardStatusAsync(cardNumber, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get card status from ESC Router for: {CardNumber}", cardNumber);
            throw;
        }
    }

    public async Task<StudentCard?> UpdateCardAsync(
        string cardNumber, 
        string cardStatusType, 
        DateTime? expiresAt = null, 
        CancellationToken cancellationToken = default)
    {
        var card = await _context.StudentCards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);

        if (card == null)
        {
            _logger.LogWarning("Card not found for update: {CardNumber}", cardNumber);
            return null;
        }

        // Update local database
        card.CardStatusType = cardStatusType;
        if (expiresAt.HasValue)
        {
            card.ExpiresAt = expiresAt.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated local card: {CardNumber}", cardNumber);

        // Update in ESC Router
        try
        {
            var request = new EscCardRequest
            {
                CardNumber = card.CardNumber,
                PersonIdentifier = card.PersonIdentifier,
                IssuerIdentifier = card.IssuerIdentifier,
                CardStatusType = card.CardStatusType,
                DisplayName = card.DisplayName,
                IssuedAt = card.IssuedAt.ToString("yyyy-MM-dd"),
                ExpiresAt = card.ExpiresAt?.ToString("yyyy-MM-dd"),
                CardType = "SMART_CDZ"
            };

            await _escRouterClient.UpdateCardAsync(cardNumber, request, cancellationToken);
            _logger.LogInformation("Successfully updated card in ESC Router: {CardNumber}", cardNumber);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update card in ESC Router. Local changes saved.");
        }

        return card;
    }

    public async Task<bool> RevokeCardAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        var card = await _context.StudentCards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);

        if (card == null)
        {
            _logger.LogWarning("Card not found for revocation: {CardNumber}", cardNumber);
            return false;
        }

        // Update local status to REVOKED
        card.CardStatusType = "REVOKED";
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Revoked local card: {CardNumber}", cardNumber);

        // Delete from ESC Router
        try
        {
            var success = await _escRouterClient.DeleteCardAsync(cardNumber, cancellationToken);
            if (success)
            {
                _logger.LogInformation("Successfully deleted card from ESC Router: {CardNumber}", cardNumber);
            }
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete card from ESC Router. Local status updated to REVOKED.");
            return false;
        }
    }

    public async Task<byte[]?> GetQrCodeAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var qrCode = await _escRouterClient.GetQrCodeAsync(cardNumber, cancellationToken);
            if (qrCode != null)
            {
                _logger.LogInformation("Retrieved QR code for card: {CardNumber}, Size: {Size} bytes", cardNumber, qrCode.Length);
            }
            return qrCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get QR code for card: {CardNumber}", cardNumber);
            throw;
        }
    }

  
    private static string GenerateEsi(int studentId)
    {
        return $"urn:schac:personalUniqueCode:int:esi:MK:UKIM:{studentId}";
    }
}