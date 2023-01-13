using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PhonemeType
    {
        None,
        Any,

        Plosive,
        Nasal,
        Trill,
        Tap,
        SibilantFricative,
        Fricative,
        LateralFricative,
        Approximant,
        LateralApproximant,

        Closed,
        ClosedClosedMiddle,
        ClosedMiddle,
        Middle,
        OpenMiddle,
        OpenOpenMiddle,
        Open
    }
}