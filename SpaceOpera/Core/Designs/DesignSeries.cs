namespace SpaceOpera.Core.Designs
{
    public class DesignSeries
    {
        public string Name { get; private set; }

        private List<DesignConfiguration> _Designs = new List<DesignConfiguration>();

        public DesignSeries(DesignConfiguration InitialDesign)
        {
            _Designs.Add(InitialDesign);
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public DesignTemplate GetDesignTemplate()
        {
            return _Designs.First().Template;
        }

        public Dictionary<SegmentTemplate, SegmentConfiguration> GetSegmentConfiguration()
        {
            return _Designs.First().GetSegments().ToDictionary(x => x.Template, x => x.Configuration);
        }
    }
}