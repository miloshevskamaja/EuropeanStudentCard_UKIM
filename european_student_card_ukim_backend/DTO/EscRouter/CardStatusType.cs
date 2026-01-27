using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardStatusType
{
    [EnumMember(Value = "ACTIVE")]
    Active,

    [EnumMember(Value = "INACTIVE")]
    Inactive,

    [EnumMember(Value = "EXPIRED")]
    Expired,

    [EnumMember(Value = "PENDING")]
    Pending,

    [EnumMember(Value = "REVOKED")]
    Revoked
}