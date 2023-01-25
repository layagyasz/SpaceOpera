using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PhonemeClass
    {
        None,
        Any,

        Consonant,
        Vowel
    }
}
