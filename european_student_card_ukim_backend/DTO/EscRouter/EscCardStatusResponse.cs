using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

public class EscCardStatusResponse
{
    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; } = string.Empty;

    [JsonPropertyName("cardStatusType")]
    public string CardStatusType { get; set; } = string.Empty;

    [JsonPropertyName("valid")]
    public bool Valid { get; set; }
}