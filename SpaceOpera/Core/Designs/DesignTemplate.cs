using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    public class DesignTemplate : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public ComponentType Type { get; set; }
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public Structure? Structure { get; set; }
        public MultiCount<ComponentTag> Tags { get; set; } = new();
        public EnumMap<ComponentSize, BaseComponent> Sizes { get; set; } = new();
        public SegmentTemplate[] Segments { get; set; } = Array.Empty<SegmentTemplate>();
    }
}
