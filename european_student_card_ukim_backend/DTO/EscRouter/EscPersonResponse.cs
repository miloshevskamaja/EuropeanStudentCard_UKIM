using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

public class EscPersonResponse
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("identifierCode")]
    public string IdentifierCode { get; set; } = string.Empty;

    [JsonPropertyName("personOrganisationUpdateViews")]
    public List<PersonOrganisationUpdateView> PersonOrganisationUpdateViews { get; set; } = new();
}
