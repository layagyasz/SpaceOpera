using Cardamom.Collections;

namespace SpaceOpera.Core.Languages
{
    public class PhonemeRange
    {
        public EnumSet<PhonemeClass> Classes { get; set; } = new();
        public EnumSet<PhonemeVoice> Voices { get; set; } = new();
        public EnumSet<PhonemeType> Types { get; set; } = new();
        public EnumSet<PhonemePosition> Positions { get; set; } = new();

        public static PhonemeRange CreateEmpty()
        {
            return new PhonemeRange();
        }

        public static PhonemeRange CreateFull()
        {
            return new PhonemeRange()
            {
                Classes = new(PhonemeClass.Any),
                Voices = new(PhonemeVoice.Any),
                Types = new(PhonemeType.Any),
                Positions = new(PhonemePosition.Any)
            };
        }

        public PhonemeRange Union(PhonemeRange other)
        {
            return new PhonemeRange() {
                Classes = new EnumSet<PhonemeClass>(Classes.Union(other.Classes)),
                Voices = new EnumSet<PhonemeVoice>(Voices.Union(other.Voices)),
                Types = new EnumSet<PhonemeType>(Types.Union(other.Types)),
                Positions = new EnumSet<PhonemePosition>(Positions.Union(other.Positions))
            };
        }

        public bool Contains(PhonemeRange range)
        {
            return Classes.Any(x => range.Classes.Any(y => PhonemeUtils.Contains(x, y)))
                && Voices.Any(x => range.Voices.Any(y => PhonemeUtils.Contains(x, y)))
                && Types.Any(x => range.Types.Any(y => PhonemeUtils.Contains(x, y)))
                && Positions.Any(x => range.Positions.Any(y => PhonemeUtils.Contains(x, y)));
        }

        public float Distance(PhonemeRange other)
        {
            return Classes.Min(x => other.Classes.Min(y => PhonemeUtils.Distance(x, y)))
                + Voices.Min(x => other.Voices.Min(y => PhonemeUtils.Distance(x, y)))
                + Types.Min(x => other.Types.Min(y => PhonemeUtils.Distance(x, y)))
                + Positions.Min(x => other.Positions.Min(y => PhonemeUtils.Distance(x, y)));
        }
    }
}