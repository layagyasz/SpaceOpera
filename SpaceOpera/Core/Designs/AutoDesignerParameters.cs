using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public class AutoDesignerParameters
    {
        public ComponentType Type { get; set; }
        public EnumMap<ComponentAttribute, float> AttributeFitness { get; set; } = new();

        public float GetFitness(IComponent component)
        {
            return AttributeFitness == null ? 0 : AttributeFitness.Sum(x => x.Value * component.GetAttribute(x.Key));
        }

        public float GetFitness(Segment Segment)
        {
            return Segment.GetComponents().Sum(x => x.Value.Sum(y => GetFitness(y)));
        }
    }
}