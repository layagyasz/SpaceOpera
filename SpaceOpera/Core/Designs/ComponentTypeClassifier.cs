using Cardamom.Collections;

namespace SpaceOpera.Core.Designs
{
    public class ComponentTypeClassifier
    {
        public class ClassificationOption
        {
            public List<ComponentTag> Tags { get; set; } = new();
            public EnumMap<ComponentTag, float> Fit { get; set; } = new();

            public float GetFit(DesignConfiguration Design)
            {
                return Design.GetTags().Select(x => Fit[x]).DefaultIfEmpty(0).Sum();
            }
        }

        public EnumSet<ComponentType> SupportedTypes { get; set; } = new();
        public EnumSet<ComponentTag> ReducedTags { get; set; } = new();
        public List<List<ClassificationOption>> ClassificationOptions { get; set; } = new();

        public IEnumerable<ComponentTag> GetTags(DesignConfiguration design)
        {
            return ClassificationOptions
                .SelectMany(x => x.ArgMax(y => y.GetFit(design)).Tags)
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