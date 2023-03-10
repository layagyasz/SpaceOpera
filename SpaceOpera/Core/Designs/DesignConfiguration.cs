namespace SpaceOpera.Core.Designs
{
    public class DesignConfiguration
    {
        public string Name { get; private set; } = string.Empty;
        public DesignTemplate Template { get; set; }

        private readonly List<Segment> _segments;

        public DesignConfiguration(DesignTemplate template, IEnumerable<Segment> segments)
        {
            Template = template;
            _segments = segments.ToList();
        }

        public IEnumerable<ComponentAndSlot> GetComponents()
        {
            return _segments.SelectMany(x => x.GetComponents())
                .SelectMany(x => x.Value.Select(y => new ComponentAndSlot(x.Key, y)));
        }

        public IEnumerable<Segment> GetSegments()
        {
            return _segments;
        }

        public IEnumerable<ComponentTag> GetTags()
        {
            return Enumerable.Concat(Template.Tags, _segments.SelectMany(x => x.GetTags()));
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public bool Validate()
        {
            return _segments.All(x => x.Validate());
        }
    }
}
