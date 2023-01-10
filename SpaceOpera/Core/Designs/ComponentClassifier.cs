using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class ComponentClassifier
    {
        public List<ComponentTypeClassifier> Classifiers { get; }

        public ComponentClassifier(IEnumerable<ComponentTypeClassifier> Classifiers)
        {
            this.Classifiers = Classifiers.ToList();
        }

        public IEnumerable<ComponentTag> Classify(DesignConfiguration Design)
        {
            var classifier = Classifiers.FirstOrDefault(x => x.Supports(Design));
            if (classifier == null)
            {
                return Design.GetTags();
            }
            var tags = classifier.GetTags(Design);
            return classifier.ReduceTags(Design.GetTags()).Concat(tags);
        }
    }
}