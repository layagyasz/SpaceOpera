using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages
{
    class PhonemeRange
    {
        [JsonConverter(typeof(EnumSetJsonConverter<PhonemeClass>))]
        public EnumSet<PhonemeClass> Classes { get; set; }

        [JsonConverter(typeof(EnumSetJsonConverter<PhonemeVoice>))]
        public EnumSet<PhonemeVoice> Voices { get; set; }

        [JsonConverter(typeof(EnumSetJsonConverter<PhonemeType>))]
        public EnumSet<PhonemeType> Types { get; set; }

        [JsonConverter(typeof(EnumSetJsonConverter<PhonemePosition>))]
        public EnumSet<PhonemePosition> Positions { get; set; }

        public static PhonemeRange CreateEmpty()
        {
            return new PhonemeRange()
            {
                Classes = new EnumSet<PhonemeClass>(),
                Voices = new EnumSet<PhonemeVoice>(),
                Types = new EnumSet<PhonemeType>(),
                Positions = new EnumSet<PhonemePosition>()
            };
        }

        public PhonemeRange Union(PhonemeRange Other)
        {
            return new PhonemeRange() {
                Classes = new EnumSet<PhonemeClass>(Classes.Union(Other.Classes)),
                Voices = new EnumSet<PhonemeVoice>(Voices.Union(Other.Voices)),
                Types = new EnumSet<PhonemeType>(Types.Union(Other.Types)),
                Positions = new EnumSet<PhonemePosition>(Positions.Union(Other.Positions))
            };
        }

        public bool Contains(PhonemeRange Range)
        {
            return Classes.Any(x => Range.Classes.Any(y => PhonemeUtils.Contains(x, y)))
                && Voices.Any(x => Range.Voices.Any(y => PhonemeUtils.Contains(x, y)))
                && Types.Any(x => Range.Types.Any(y => PhonemeUtils.Contains(x, y)))
                && Positions.Any(x => Range.Positions.Any(y => PhonemeUtils.Contains(x, y)));
        }

        public float Distance(PhonemeRange Other)
        {
            return Classes.Min(x => Other.Classes.Min(y => PhonemeUtils.Distance(x, y)))
                + Voices.Min(x => Other.Voices.Min(y => PhonemeUtils.Distance(x, y)))
                + Types.Min(x => Other.Types.Min(y => PhonemeUtils.Distance(x, y)))
                + Positions.Min(x => Other.Positions.Min(y => PhonemeUtils.Distance(x, y)));
        }
    }
}