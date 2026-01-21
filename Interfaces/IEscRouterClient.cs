using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Interfaces
{
    public interface IEscRouterClient
    {
        // Now passing the full card object or necessary fields to match V2 payload structure
        Task<bool> IssueCardAsync(StudentCard card);
        Task<string> GenerateEscnAsync();
        Task<string> CheckCardStatusAsync(string cardNumber);
        // TODO: Implement QR code retrieval for a given ESCN (card number)
        // This method should return the raw PNG bytes (byte[]) so callers can stream the image.
        Task<byte[]> GetQrCodeAsync(string cardNumber);
        // Additional needed ESC Router operations
        Task<bool> UpdateCardAsync(StudentCard card);
        Task<bool> RevokeCardAsync(string cardNumber);
        Task<StudentCard?> GetCardAsync(string cardNumber);
    }
}
