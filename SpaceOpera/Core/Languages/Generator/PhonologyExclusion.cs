using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class PhonologyExclusion
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum PhonologySegmentType
        {
            ONSET,
            NUCLEUS,
            OFFSET
        }

        public PhonologySegmentType Segment { get; set; }
        public PhonemeRange Left { get; set; }
        public PhonemeRange Right { get; set; }
    }
}