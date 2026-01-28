using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

public class EscPersonRequest
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("identifierCode")]
    public string IdentifierCode { get; set; } = "ESI";

    [JsonPropertyName("personOrganisationUpdateViews")]
    public List<PersonOrganisationUpdateView> PersonOrganisationUpdateViews { get; set; } = new();
}