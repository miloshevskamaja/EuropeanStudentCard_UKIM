using Microsoft.EntityFrameworkCore;
using EuropeanStudentCard.Data;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Services
{
    public class CardService : ICardService
    {
        private readonly AppDbContext _context;
        private readonly IEscRouterClient _escRouterClient;

        public CardService(AppDbContext context, IEscRouterClient escRouterClient)
        {
            _context = context;
            _escRouterClient = escRouterClient;
        }

        public async Task<StudentCard> GenerateCardAsync(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }

            // Check if card already exists
            var existingCard = await _context.StudentCards
                .FirstOrDefaultAsync(c => c.StudentId == studentId);
            if (existingCard != null)
            {
                return existingCard;
            }

            // 1. Generate Card Number (ESCN) via ESC Router API
            string cardNumber = await _escRouterClient.GenerateEscnAsync();

            // 2. Generate Person Identifier (ESI) - e.g. urn:schac:personalUniqueCode:int:esi:MK:UKIM:{studentId}
            string personIdentifier = $"urn:schac:personalUniqueCode:int:esi:MK:UKIM:{studentId}";
            var displayName = "Card of " + student.Name;
            // 3. Create Card Record
            var newCard = new StudentCard
            {
                CardNumber = cardNumber,
                PersonIdentifier = personIdentifier,
                IssuerIdentifier = "999888777", 
                CardStatusType = "PENDING",
                DisplayName = displayName,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(4),
                StudentId = studentId
            };

            _context.StudentCards.Add(newCard);
            await _context.SaveChangesAsync();

            // 4. Register with External Router (Mocked)
            // Now we pass the full object as required by V2 logic
            bool success = await _escRouterClient.IssueCardAsync(newCard);
            if (success)
            {
                newCard.CardStatusType = "ACTIVE";
                await _context.SaveChangesAsync();
            }

            return newCard;
        }

        public async Task<StudentCard?> GetCardStatusAsync(string cardNumber)
        {
            return await _context.StudentCards
                .Include(c => c.Student)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }
    }
}
