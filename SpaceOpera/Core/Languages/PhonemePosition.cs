using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PhonemePosition
    {
        None,
        Any,

        Bilabial,
        Labiodental,
        Dental,
        Alveolar,
        Postalveolar,
        Retroflex,
        Palatal,
        Velar,
        Uvular,
        Pharyngeal,
        Glottal,
        Labiovelar,

        Front,
        Central,
        Back
    }
}