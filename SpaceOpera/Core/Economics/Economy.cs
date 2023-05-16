using Cardamom;
using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class Economy : ITickable
    {
        public AdvancementManager AdvancementManager { get; }
        public MaterialSink MaterialSink { get; }

        private readonly Dictionary<CompositeKey<Faction, StellarBody>, StellarBodyHolding> _holdings = new();
        private readonly List<PersistentRoute> _routes = new();
        private readonly List<Trade> _trades = new();

        public Economy(AdvancementManager advancementManager, MaterialSink materialSink)
        {
            AdvancementManager = advancementManager;
            MaterialSink = materialSink;
        }

        public StellarBodyRegionHolding CreateSovereignHolding(Faction faction, StellarBodyRegion stellarBodyRegion)
        {
            var holding = GetOrCreateHolding(faction, stellarBodyRegion.Parent!);
            var regionHolding = new StellarBodyRegionHolding(holding, stellarBodyRegion);
            holding.AddSubzone(stellarBodyRegion, regionHolding);
            regionHolding.AddStructureNodes((int)stellarBodyRegion.StructureNodes);
            foreach (var resource in stellarBodyRegion.Resources)
            {
                regionHolding.AddResourceNodes(Count<ResourceNode>.Create(resource, resource.Size));
            }
            return regionHolding;
        }

        public StellarBodyHolding? GetHolding(Faction faction, StellarBody stellarBody)
        {
            _holdings.TryGetValue(
                CompositeKey<Faction, StellarBody>.Create(faction, stellarBody), out var holding);
            return holding;
        }

        public StellarBodyRegionHolding? GetHolding(Faction faction, StellarBodyRegion stellarBodyRegion)
        {
            var holding = GetHolding(faction, stellarBodyRegion.Parent!);
            if (holding != null)
            {
                return (StellarBodyRegionHolding?)holding.GetSubzone(stellarBodyRegion);
            }
            return null;
        }

        public IEnumerable<StellarBodyHolding> GetHoldingsFor(Faction faction)
        {
            return _holdings.Where(x => x.Key.Key1 == faction).Select(x => x.Value).Cast<StellarBodyHolding>();
        }

        public IEnumerable<PersistentRoute> GetPersistentRoutesFor(Faction faction)
        {
            return _routes.Where(x => x.Faction == faction);
        }

        public IEnumerable<StellarBodyRegionHolding> GetSubzoneHoldings(Faction faction)
        {
            return GetHoldingsFor(faction).SelectMany(x => x.GetSubzones()).Cast<StellarBodyRegionHolding>();
        }

        public void Tick()
        {
            foreach (var holding in _holdings.Values)
            {
                holding.Tick();
            }

            _routes.ForEach(x => x.Tick());
            _trades.ForEach(x => x.Tick());

            foreach (var holding in _holdings.Values)
            {
                holding.Consume(MaterialSink);
            }
        }

        private StellarBodyHolding GetOrCreateHolding(Faction faction, StellarBody stellarBody)
        {
            var holding = GetHolding(faction, stellarBody);
            if (holding == null)
            {
                holding = new StellarBodyHolding(faction, AdvancementManager.Get(faction), stellarBody);
                _holdings.Add(CompositeKey<Faction, StellarBody>.Create(faction, stellarBody), holding);
            }
            return holding;
        }
    }
}