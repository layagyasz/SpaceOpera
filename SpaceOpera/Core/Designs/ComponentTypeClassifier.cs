using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public class ComponentTypeClassifier
    {
        public class ClassificationOption
        {
            public ComponentTag[] Tags { get; set; } = Array.Empty<ComponentTag>();
            public DesignFitness Fit { get; set; } = new();

            public float GetFit(IComponent component)
            {
                return Fit.Get(component);
            }
        }

        public EnumSet<ComponentType> SupportedTypes { get; set; } = new();
        public EnumSet<ComponentTag> ReducedTags { get; set; } = new();
        public ClassificationOption[][] ClassificationOptions { get; set; } = Array.Empty<ClassificationOption[]>();

        public IEnumerable<ComponentTag> Classify(IComponent component)
        {
            var newTags = ClassificationOptions
                .SelectMany(x => x.ArgMax(y => y.GetFit(component))!.Tags)
                .Distinct();
            return component.Tags.Select(x => x.Key).Where(x => !ReducedTags.Contains(x)).Concat(newTags);
        }

        public bool Supports(IComponent component)
        {
            return SupportedTypes.Contains(component.Slot.Type);
        }
    }
}