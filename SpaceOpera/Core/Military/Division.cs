using Cardamom.Trackers;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public class Division : BaseFormation
    {
        public DivisionTemplate Template { get; }
        public StellarBody? StellarBodyLocation { get; private set; }

        public Division(string name, Faction faction, DivisionTemplate template, MultiCount<Unit> composition)
            : base(faction)
        {
            SetName(name);
            Template = template;
        }
    }
}