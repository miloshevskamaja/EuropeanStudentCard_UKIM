using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;


public class EscCardRequest
{
    // ESCN
    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; } = string.Empty;
 
    [JsonPropertyName("personIdentifier")]
    public string PersonIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("issuerIdentifier")]
    public string IssuerIdentifier { get; set; } = string.Empty;

    [JsonPropertyName("cardStatusType")]
    public string CardStatusType { get; set; } = "PENDING";

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("issuedAt")]
    public string IssuedAt { get; set; } = string.Empty;

    [JsonPropertyName("expiresAt")]
    public string? ExpiresAt { get; set; }

    [JsonPropertyName("cardType")]
    public string CardType { get; set; } = "SMART_CDZ";
}