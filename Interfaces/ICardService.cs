using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Interfaces
{
    public interface ICardService
    {
        Task<StudentCard?> GetCardStatusAsync(string cardNumber);
        Task<StudentCard> GenerateCardAsync(int studentId);
    }
}
