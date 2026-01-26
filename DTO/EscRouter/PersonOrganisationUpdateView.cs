using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

public class PersonOrganisationUpdateView
{
    [JsonPropertyName("academicLevel")]
    public string AcademicLevel { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("organisationIdentifier")]
    public string OrganisationIdentifier { get; set; } = string.Empty;
}