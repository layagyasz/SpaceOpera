namespace SpaceOpera.Core.Designs
{
    public class ComponentClassifier
    {
        public ComponentTypeClassifier[] Classifiers { get; }

        public ComponentClassifier(IEnumerable<ComponentTypeClassifier> classifiers)
        {
            Classifiers = classifiers.ToArray();
        }

        public IEnumerable<ComponentTag> Classify(IComponent component)
        {
            var classifier = Classifiers.FirstOrDefault(x => x.Supports(component));
            var designTags = component.Tags;
            if (classifier == null)
            {
                return component.Tags.Select(x => x.Key);
            }
            return classifier.Classify(component);
        }
    }
}