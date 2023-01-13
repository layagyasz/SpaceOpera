namespace SpaceOpera.Core.Designs
{
    public class DesignConfiguration
    {
        public DesignTemplate Template { get; set; }

        private readonly List<Segment> _segments;

        public DesignConfiguration(DesignTemplate template, IEnumerable<Segment> segments)
        {
            Template = template;
            _segments = segments.ToList();
        }

        public IEnumerable<Segment> GetSegments()
        {
            return _segments;
        }

        public IEnumerable<ComponentTag> GetTags()
        {
            throw new NotImplementedException();
        }
    }
}
