using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentSize
    {
        Unknown,
        Tiny,
        ExtraSmall,
        PointDefense,
        Small,
        Medium,
        Large,
    }
}
