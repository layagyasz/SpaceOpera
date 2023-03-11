using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentSize
    {
        Unknown,
        Small,
        Medium,
        Large,
    }
}
