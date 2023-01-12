namespace SpaceOpera.Core.Designs
{
    public class ComponentClassifier
    {
        public List<ComponentTypeClassifier> Classifiers { get; }

        public ComponentClassifier(IEnumerable<ComponentTypeClassifier> classifiers)
        {
            Classifiers = classifiers.ToList();
        }

        public IEnumerable<ComponentTag> Classify(DesignConfiguration design)
        {
            var classifier = Classifiers.FirstOrDefault(x => x.Supports(design));
            if (classifier == null)
            {
                return design.GetTags();
            }
            var tags = classifier.GetTags(design);
            return classifier.ReduceTags(design.GetTags()).Concat(tags);
        }
    }
}