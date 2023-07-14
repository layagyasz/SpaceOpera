using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Government
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Succession
    {
        Elected,
        Inherited,
        Selected
    }
}
