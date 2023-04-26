using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public class Division : BaseFormation
    {
        public DivisionTemplate Template { get; }
        public StellarBody? StellarBodyLocation { get; private set; }

        public Division(Faction faction, DivisionTemplate template)
            : base(faction)
        {
            Template = template;
        }
    }
}