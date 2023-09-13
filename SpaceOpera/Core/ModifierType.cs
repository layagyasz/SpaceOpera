using System.Text.Json.Serialization;

namespace SpaceOpera.Core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModifierType
    {
        Unknown,

        Diplomatic,
        PopulationGeneration,
        ResourceGeneration
    }
}
