using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Clients.EscClients
{

    public interface IEscRouterClient
    {
        // Card Endpoints 
        Task<string> GenerateEscnAsync(CancellationToken cancellationToken = default);
        Task<EscCardResponse?> CreateCardAsync(EscCardRequest request, CancellationToken cancellationToken = default);
        Task<List<EscCardResponse>> GetAllCardsAsync(CancellationToken cancellationToken = default);
        Task<EscCardResponse?> GetCardAsync(string escn, CancellationToken cancellationToken = default);
        Task<EscCardResponse?> UpdateCardAsync(string escn, EscCardRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteCardAsync(string escn, CancellationToken cancellationToken = default);
        Task<byte[]?> GetQrCodeAsync(string escn, CancellationToken cancellationToken = default);
        Task<EscCardStatusResponse?> GetCardStatusAsync(string escn, CancellationToken cancellationToken = default);

        // Person Endpoints 
        Task<List<EscPersonResponse>> GetAllPersonsAsync(CancellationToken cancellationToken = default);
        Task<EscPersonResponse?> CreatePersonAsync(EscPersonRequest request, CancellationToken cancellationToken = default);
        Task<EscPersonResponse?> GetPersonAsync(string esi, CancellationToken cancellationToken = default);
        Task<EscPersonResponse?> UpdatePersonAsync(string esi, EscPersonRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeletePersonAsync(string esi, CancellationToken cancellationToken = default);
    }
}
