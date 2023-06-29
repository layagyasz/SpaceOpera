using Cardamom.Collections;
using Cardamom.Trackers;

namespace SpaceOpera.Core.Designs
{
    public class ComponentTypeClassifier
    {
        public class ClassificationOption
        {
            public ComponentTag[] Tags { get; set; } = Array.Empty<ComponentTag>();
            public EnumMap<ComponentTag, float> Fit { get; set; } = new();

            public float GetFit(MultiCount<ComponentTag> tags)
            {
                return tags.Select(x => x.Value * Fit[x.Key]).DefaultIfEmpty(0).Sum();
            }
        }

        public EnumSet<ComponentType> SupportedTypes { get; set; } = new();
        public EnumSet<ComponentTag> ReducedTags { get; set; } = new();
        public ClassificationOption[][] ClassificationOptions { get; set; } = Array.Empty<ClassificationOption[]>();

        public IEnumerable<ComponentTag> GetTags(DesignConfiguration design)
        {
            var tags = design.GetTags();
            return ClassificationOptions
                .SelectMany(x => x.ArgMax(y => y.GetFit(tags))!.Tags)
                .Distinct();
        }

        public IEnumerable<ComponentTag> ReduceTags(IEnumerable<ComponentTag> tags)
        {
            return tags.Where(x => !ReducedTags.Contains(x));
        }

        public bool Supports(DesignConfiguration design)
        {
            return SupportedTypes.Contains(design.Template.Type);
        }
    }
}