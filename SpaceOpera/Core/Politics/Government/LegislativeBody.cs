using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Government
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LegislativeBody
    {
        Direct,
        RepresentativeMajority,
        RepresentativeUnanimous,
        Council,
        Single
    }
}
