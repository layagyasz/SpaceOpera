using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    struct DesignSlot
    {
        [JsonConverter(typeof(EnumSetJsonConverter<ComponentType>))]
        public EnumSet<ComponentType> Type { get; set; }
        [JsonConverter(typeof(EnumSetJsonConverter<ComponentSize>))]
        public EnumSet<ComponentSize> Size { get; set; }
        public int Count { get; set; }
        public int Weight { get; set; }
    }
}