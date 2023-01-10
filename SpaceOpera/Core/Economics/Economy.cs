using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class Economy : ITickable
    {
        public MaterialSink MaterialSink { get; }

        private Dictionary<CompositeKey<Faction, StellarBody>, StellarBodyHolding> _Holdings = 
            new Dictionary<CompositeKey<Faction, StellarBody>, StellarBodyHolding>();
        private readonly List<PersistentRoute> _Routes = new List<PersistentRoute>();
        private readonly List<Trade> _Trades = new List<Trade>();

        public Economy(MaterialSink MaterialSink)
        {
            this.MaterialSink = MaterialSink;
        }

        public void CreateSovereignHolding(Faction Faction, StellarBodyRegion StellarBodyRegion)
        {
            var holding = GetOrCreateHolding(Faction, StellarBodyRegion.Parent);
            var regionHolding = new StellarBodyRegionHolding(holding, StellarBodyRegion);
            holding.AddSubzone(StellarBodyRegion, regionHolding);
            regionHolding.AddStructureNodes((int)StellarBodyRegion.StructureNodes);
            foreach (ResourceNode resource in StellarBodyRegion.Resources)
            {
                regionHolding.AddResourceNodes(new Count<ResourceNode>(resource, resource.Size));
            }
        }

        public StellarBodyHolding GetHolding(Faction Faction, StellarBody StellarBody)
        {
            _Holdings.TryGetValue(
                CompositeKey<Faction, StellarBody>.Create(Faction, StellarBody), out StellarBodyHolding holding);
            return holding;
        }

        public StellarBodyRegionHolding GetHolding(Faction Faction, StellarBodyRegion StellarBodyRegion)
        {
            var holding = GetHolding(Faction, StellarBodyRegion.Parent);
            if (holding != null)
            {
                return (StellarBodyRegionHolding)holding.GetSubzone(StellarBodyRegion);
            }
            return null;
        }

        public IEnumerable<StellarBodyRegionHolding> GetHoldings(Faction Faction)
        {
            return _Holdings
                .Where(x => x.Key.Key1 == Faction)
                .SelectMany(x => x.Value.GetSubzones())
                .Cast<StellarBodyRegionHolding>();
        }

        public void Tick()
        {
            foreach (var holding in _Holdings.Values)
            {
                holding.Tick();
            }

            _Routes.ForEach(x => x.Tick());
            _Trades.ForEach(x => x.Tick());

            foreach (var holding in _Holdings.Values)
            {
                holding.Consume(MaterialSink);
            }
        }

        private StellarBodyHolding GetOrCreateHolding(Faction Faction, StellarBody StellarBody)
        {
            var holding = GetHolding(Faction, StellarBody);
            if (holding == null)
            {
                holding = new StellarBodyHolding(Faction, StellarBody);
                _Holdings.Add(CompositeKey<Faction, StellarBody>.Create(Faction, StellarBody), holding);
            }
            return holding;
        }
    }
}