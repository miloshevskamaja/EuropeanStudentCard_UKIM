using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Interfaces
{
    /// <summary>
    /// Interface for ESC Router V2 API client.
    /// Implements all card and person management endpoints.
    /// </summary>
    public interface IEscRouterClient
    {
        // ==================== Card Endpoints ====================
        
        /// <summary>
        /// Generate a new European Student Card Number (ESCN).
        /// GET /api/v2/cards/generate-escn
        /// </summary>
        Task<string> GenerateEscnAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Create/issue a new card.
        /// POST /api/v2/cards
        /// </summary>
        Task<EscCardResponse?> CreateCardAsync(EscCardRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get all cards for the organization.
        /// GET /api/v2/cards
        /// </summary>
        Task<List<EscCardResponse>> GetAllCardsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get card details by ESCN.
        /// GET /api/v2/cards/{escn}
        /// </summary>
        Task<EscCardResponse?> GetCardAsync(string escn, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Update an existing card.
        /// PUT /api/v2/cards/{escn}
        /// </summary>
        Task<EscCardResponse?> UpdateCardAsync(string escn, EscCardRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Delete/revoke a card.
        /// DELETE /api/v2/cards/{escn}
        /// </summary>
        Task<bool> DeleteCardAsync(string escn, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get QR code image for a card.
        /// GET /api/v2/cards/{escn}/qr
        /// </summary>
        Task<byte[]?> GetQrCodeAsync(string escn, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Check card status and validity.
        /// GET /api/v2/cards/{escn}/status
        /// </summary>
        Task<EscCardStatusResponse?> GetCardStatusAsync(string escn, CancellationToken cancellationToken = default);
        
        // ==================== Person Endpoints ====================
        
        /// <summary>
        /// Get all persons registered in the organization.
        /// GET /api/v2/persons
        /// </summary>
        Task<List<EscPersonResponse>> GetAllPersonsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Register a new person (student).
        /// POST /api/v2/persons
        /// </summary>
        Task<EscPersonResponse?> CreatePersonAsync(EscPersonRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get person by ESI.
        /// GET /api/v2/persons/{esi}
        /// </summary>
        Task<EscPersonResponse?> GetPersonAsync(string esi, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Update person information.
        /// PUT /api/v2/persons/{esi}
        /// </summary>
        Task<EscPersonResponse?> UpdatePersonAsync(string esi, EscPersonRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Delete a person.
        /// DELETE /api/v2/persons/{esi}
        /// </summary>
        Task<bool> DeletePersonAsync(string esi, CancellationToken cancellationToken = default);
    }
}
