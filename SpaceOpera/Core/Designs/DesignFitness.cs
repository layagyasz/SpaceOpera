using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public class DesignFitness
    {
        public EnumMap<ComponentAttribute, float> Attributes { get; set; } = new();
        public EnumMap<ComponentTag, float> Tags { get; set; } = new();

        public float Get(IComponent component)
        {
            return Attributes.Sum(x => x.Value * component.GetAttribute(x.Key))
                + component.Tags.Sum(x => x.Value * Tags[x.Key]);
        }

        public float Get(Segment segment)
        {
            return segment.GetComponents().Sum(x => x.Value.Sum(y => Get(y)))
                + segment.GetTags().Sum(x => x.Value * Tags[x.Key]);
        }
    }
}
