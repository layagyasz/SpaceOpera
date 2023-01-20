using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Designs
{
    public class DesignLicense
    {
        public Faction Faction { get; }
        public Design Design { get; }

        public DesignLicense(Faction faction, Design design)
        {
            Faction = faction;
            Design = design;
        }
    }
}
