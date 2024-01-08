using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NameType
    {
        Army,
        BattalionTemplate,
        Division,
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
        Vehicle,
        Weapon
    }
}
