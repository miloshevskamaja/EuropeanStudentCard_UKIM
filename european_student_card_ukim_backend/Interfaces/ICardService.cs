using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Interfaces
{
    public interface ICardService
    {
        Task<StudentCard> GenerateCardAsync(int studentId, CancellationToken cancellationToken = default);
        Task<List<StudentCard>> GetAllCardsAsync(CancellationToken cancellationToken = default);
        Task<StudentCard?> GetCardByNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
        Task<EscCardStatusResponse?> GetCardStatusFromRouterAsync(string cardNumber, CancellationToken cancellationToken = default);
        Task<StudentCard?> UpdateCardAsync(string cardNumber, string cardStatusType, DateTime? expiresAt = null, CancellationToken cancellationToken = default);
        Task<bool> RevokeCardAsync(string cardNumber, CancellationToken cancellationToken = default);
        Task<byte[]?> GetQrCodeAsync(string cardNumber, CancellationToken cancellationToken = default);
    }
}
