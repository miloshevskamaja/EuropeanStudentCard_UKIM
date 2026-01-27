using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AcademicLevel
{
    Bachelor,
    Master,
    Doctorate
}