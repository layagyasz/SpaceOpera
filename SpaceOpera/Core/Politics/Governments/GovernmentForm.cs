using Cardamom;
using SpaceOpera.Core.Politics.Cultures;

namespace SpaceOpera.Core.Politics.Governments
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
        public CulturalTraitsRange CultureRestriction { get; set; } = new();

        public bool IsValid(Culture culture)
        {
            return CultureRestriction.Contains(culture.Traits);
        }
    }
}
