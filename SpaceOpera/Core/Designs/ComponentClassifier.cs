namespace SpaceOpera.Core.Designs
{
    public class ComponentClassifier
    {
        public ComponentTypeClassifier[] Classifiers { get; }

        public ComponentClassifier(IEnumerable<ComponentTypeClassifier> classifiers)
        {
            Classifiers = classifiers.ToArray();
        }

        public IEnumerable<ComponentTag> Classify(DesignConfiguration design)
        {
            var classifier = Classifiers.FirstOrDefault(x => x.Supports(design));
            var designTags = design.GetTags().Select(x => x.Key);
            if (classifier == null)
            {
                return designTags;
            }
            var tags = classifier.GetTags(design);
            return classifier.ReduceTags(designTags).Concat(tags);
        }
    }
}