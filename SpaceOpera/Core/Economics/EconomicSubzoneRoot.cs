using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicSubzoneRoot
    {
        public StellarBodyRegion Region { get; }

        private readonly List<EconomicSubzoneHolding> _holdings = new();

        public EconomicSubzoneRoot(StellarBodyRegion region)
        {
            Region = region;
        }

        public void AddHolding(EconomicSubzoneHolding holding)
        {
            _holdings.Add(holding);
        }

        public EconomicSubzoneHolding? GetHolding(Faction faction)
        {
            return _holdings.FirstOrDefault(x => x.Owner == faction);
        }
    }
}
