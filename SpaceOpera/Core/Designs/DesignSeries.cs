namespace SpaceOpera.Core.Designs
{
    public class DesignSeries
    {
        public string Name { get; private set; } = string.Empty;

        private List<DesignConfiguration> _designs = new();

        public DesignSeries(DesignConfiguration initialDesign)
        {
            _designs.Add(initialDesign);
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public DesignTemplate GetDesignTemplate()
        {
            return _designs.First().Template;
        }

        public Dictionary<SegmentTemplate, SegmentConfiguration> GetSegmentConfiguration()
        {
            return _designs.First().GetSegments().ToDictionary(x => x.Template, x => x.Configuration);
        }
    }
}