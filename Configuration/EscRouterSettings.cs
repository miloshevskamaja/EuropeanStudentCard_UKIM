using System.ComponentModel.DataAnnotations;

namespace EuropeanStudentCard.Configuration;

public class EscRouterSettings
{
    [Required]
    public string BaseUrl { get; set; } = "https://router.europeanstudentcard.eu/esc-rest/api/v2";

    public string? Token { get; set; }

    /// Participant Identification Code 
    [Required]
    public string Pic { get; set; } = string.Empty;

    [Required]
    [Range(0, 999)]
    public int EscnPrefix { get; set; } = 1;

    public int TimeoutSeconds { get; set; } = 30;

    public bool HasToken => !string.IsNullOrWhiteSpace(Token);
}