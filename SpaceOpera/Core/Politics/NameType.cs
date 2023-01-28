using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NameType
    {
        BattalionTemplate,
        DivisionTemplate,
        Faction,
        Fleet,
        Infantry,
        Shield,
        Ship,
        Star,
        StarSystem,
        StellarBody,
        StellarBodyRegion,
        Weapon
    }
}
