using Cardamom.Utilities;
using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class ComponentTypeClassifier
    {
        public class ClassificationOption
        {
            public List<ComponentTag> Tags { get; set; }
            public EnumMap<ComponentTag, float> Fit { get; set; }

            public float GetFit(DesignConfiguration Design)
            {
                return Design.GetTags().Select(x => Fit[x]).DefaultIfEmpty(0).Sum();
            }
        }

        public EnumSet<ComponentType> SupportedTypes { get; set; }
        public EnumSet<ComponentTag> ReducedTags { get; set; }
        public List<List<ClassificationOption>> ClassificationOptions { get; set; }

        public IEnumerable<ComponentTag> GetTags(DesignConfiguration Design)
        {
            return ClassificationOptions
                .SelectMany(x => x.ArgMax(y => y.GetFit(Design)).Tags)
                .Distinct();
        }

        public IEnumerable<ComponentTag> ReduceTags(IEnumerable<ComponentTag> Tags)
        {
            return Tags.Where(x => !ReducedTags.Contains(x));
        }

        public bool Supports(DesignConfiguration Design)
        {
            return SupportedTypes.Contains(Design.Template.Type);
        }
    }
}