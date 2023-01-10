using SpaceOpera.Core.Economics;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class DesignTemplate : IKeyed
    {
        public string Key { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ComponentType Type { get; set; }
        public Structure Structure { get; set; }
        public List<ComponentTag> Tags { get; set; }
        [JsonConverter(typeof(EnumMapJsonConverter<ComponentSize, BaseComponent>))]
        public EnumMap<ComponentSize, BaseComponent> Sizes { get; set; }
        public List<SegmentTemplate> Segments { get; set; }
    }
}
