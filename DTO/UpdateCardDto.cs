using System.ComponentModel.DataAnnotations;

namespace EuropeanStudentCard.DTO;


public class UpdateCardDto
{
    [Required]
    public string CardStatusType { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}