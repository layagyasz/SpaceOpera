using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Economics
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MaterialType
    {
        Unknown,
        MaterialContinuous,
        MaterialDiscrete,
        Research
    }
}
