using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

public class GenerateEscnResponse
{
    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; } = string.Empty;
}