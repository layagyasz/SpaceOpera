using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Governments
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Legitimacy
    {
        Credential,
        Democratic,
        Divine,
        Military
    }
}
