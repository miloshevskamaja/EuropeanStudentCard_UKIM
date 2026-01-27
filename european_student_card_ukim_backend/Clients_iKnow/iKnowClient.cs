using EuropeanStudentCard.Models_iKnow;

namespace EuropeanStudentCard.Clients_iKnow
{
    public interface iKnowClient
    {
        Task<IKnowStudentDto?> GetStudentByIndexAsync(string index, CancellationToken ct = default);
        Task<IReadOnlyList<IKnowStudentDto>> GetActiveStudentsAsync(CancellationToken ct = default);
    }
}
