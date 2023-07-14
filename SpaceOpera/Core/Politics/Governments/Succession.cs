using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Governments
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Succession
    {
        Elected,
        Inherited,
        Selected
    }
}
