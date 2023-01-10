using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    enum PhonemeType
    {
        NONE,
        ANY,

        PLOSIVE,
        NASAL,
        TRILL,
        TAP,
        SIBILANT_FRICATIVE,
        FRICATIVE,
        LATERAL_FRICATIVE,
        APPROXIMANT,
        LATERAL_APPROXIMANT,

        CLOSED,
        CLOSED_CLOSED_MIDDLE,
        CLOSED_MIDDLE,
        MIDDLE,
        OPEN_MIDDLE,
        OPEN_OPEN_MIDDLE,
        OPEN
    }
}