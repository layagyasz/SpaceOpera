using Cardamom;

namespace SpaceOpera.Core.Politics.Government
{
    public class GovernmentForm : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Legitimacy Legitimacy { get; set; }
        public LegislativeBody LegislativeBody { get; set; }
        public Succession Succession { get; set; }
        public bool Devolved { get; set; }
        public bool Absolute { get; set; }
    }
}
