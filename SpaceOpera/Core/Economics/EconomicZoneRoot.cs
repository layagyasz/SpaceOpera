using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicZoneRoot : ITickable
    {
        public StellarBody StellarBody { get; }

        private readonly List<EconomicZoneHolding> _holdings = new();
        private readonly Dictionary<StellarBodyRegion, EconomicSubzoneRoot> _children = new();

        public EconomicZoneRoot(StellarBody stellarBody)
        {
            StellarBody = stellarBody;
        }

        public void AddChild(EconomicSubzoneRoot child)
        {
            _children.Add(child.Region, child);
        }

        public void AddHolding(EconomicZoneHolding holding)
        {
            _holdings.Add(holding);
        }

        public void Consume(MaterialSink sink)
        {
            _holdings.ForEach(x => x.Consume(sink));
        }

        public EconomicSubzoneRoot? GetChild(StellarBodyRegion region)
        {
            return _children.GetValueOrDefault(region);
        }

        public EconomicZoneHolding? GetHolding(Faction faction)
        {
            return _holdings.FirstOrDefault(x => x.Owner == faction);
        }

        public IEnumerable<EconomicZoneHolding> GetHoldings()
        {
            return _holdings;
        }

        public void Tick()
        {
            foreach (var holding in _holdings)
            {
                holding.Tick();
            }
        }
    }
}
