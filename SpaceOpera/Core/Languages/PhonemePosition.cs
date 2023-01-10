using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    enum PhonemePosition
    {
        NONE,
        ANY,

        BILABIAL,
        LABIODENTAL,
        DENTAL,
        ALVEOLAR,
        POSTALVEOLAR,
        RETROFLEX,
        PALATAL,
        VELAR,
        UVULAR,
        PHARYNGEAL,
        GLOTTAL,
        LABIOVELAR,

        FRONT,
        CENTRAL,
        BACK
    }
}