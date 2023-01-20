using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Designs
{
    public class DesignLicense
    {
        public Faction Faction { get; }
        public DesignConfiguration Design { get; }

        public DesignLicense(Faction faction, DesignConfiguration design)
        {
            Faction = faction;
            Design = design;
        }
    }
}
