namespace SpaceOpera.Core.Designs
{
    public class DesignConfiguration
    {
        public DesignTemplate Template { get; set; }
        public List<Segment> Segments { get; set; }

        public DesignConfiguration(DesignTemplate template, IEnumerable<Segment> segments)
        {
            Template = template;
            Segments = segments.ToList();
        }

        public IEnumerable<ComponentTag> GetTags()
        {
            throw new NotImplementedException();
        }
    }
}
