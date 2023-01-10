using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class AutoDesignerParameters
    {
        public ComponentType Type { get; set; }
        [JsonConverter(typeof(EnumMapJsonConverter<ComponentAttribute, float>))]
        public EnumMap<ComponentAttribute, float> AttributeFitness { get; set; }

        public float GetFitness(IComponent Component)
        {
            return AttributeFitness == null ? 0 : AttributeFitness.Sum(x => x.Value * Component.GetAttribute(x.Key));
        }

        public float GetFitness(Segment Segment)
        {
            return Segment.GetComponents().Sum(x => x.Value.Sum(y => GetFitness(y)));
        }
    }
}