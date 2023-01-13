using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PhonemeVoice
    {
        None,
        Any,
        Voiced,
        Unvoiced
    }
}
