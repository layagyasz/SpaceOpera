using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages.Generator
{
    public class PhonologyExclusion
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PhonologySegmentType
        {
            Onset,
            Nucleus,
            Offset
        }

        public PhonologySegmentType Segment { get; set; }
        public PhonemeRange Left { get; set; } = PhonemeRange.CreateEmpty();
        public PhonemeRange Right { get; set; } = PhonemeRange.CreateEmpty();
    }
}