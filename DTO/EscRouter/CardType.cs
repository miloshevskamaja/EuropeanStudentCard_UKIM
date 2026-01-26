using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace EuropeanStudentCard.DTO.EscRouter;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardType
{
    [EnumMember(Value = "UNKNOWN")]
    Unknown,

    [EnumMember(Value = "PASSIVE")]
    Passive,

    [EnumMember(Value = "SMART_NO_CDZ")]
    SmartNoCdz,

    [EnumMember(Value = "SMART_CDZ")]
    SmartCdz,

    [EnumMember(Value = "SMART_MAY_SP")]
    SmartMaySp,

    [EnumMember(Value = "SMART_PASSIVE")]
    SmartPassive,

    [EnumMember(Value = "SMART_PASSIVE_EMULATION")]
    SmartPassiveEmulation
}