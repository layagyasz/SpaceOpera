using Cardamom;
using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class Economy : ITickable
    {
        public AdvancementManager AdvancementManager { get; }
        public FormationManager FormationManager { get; }
        public MaterialSink MaterialSink { get; }

        private readonly Dictionary<Faction, EconomicRoot> _roots = new();
        private readonly Dictionary<CompositeKey<Faction, StellarBody>, StellarBodyHolding> _holdings = new();
        private readonly List<PersistentRoute> _routes = new();
        private readonly List<Trade> _trades = new();

        public Economy(
            AdvancementManager advancementManager, FormationManager formationManager, MaterialSink materialSink)
        {
            AdvancementManager = advancementManager;
            FormationManager = formationManager;
            MaterialSink = materialSink;
        }

        public void Add(Faction faction)
        {
            _roots.Add(faction, new EconomicRoot(faction));
        }

        public void AddPersistentRoute(PersistentRoute route)
        {
            lock (_routes)
            {
                _routes.Add(route);
            }
        }

        public void AddTrade(Trade trade)
        {
            lock (_trades)
            {
                _trades.Add(trade);
            }
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

        public void RemovePersistentRoute(PersistentRoute route)
        {
            lock (_routes)
            {
                _routes.Remove(route);
            }
        }

        public void RemoveTrade(Trade trade)
        {
            lock(_trades)
            {
                _trades.Remove(trade);
            }
        }

        public void Tick()
        {
            foreach (var holding in _holdings.Values)
            {
                holding.Tick();
            }

            lock (_routes)
            {
                foreach (var route in _routes)
                {
                    foreach (var fleet in route.AssignedFleets)
                    {
                        ((FleetDriver)FormationManager.GetDriver(fleet)).SetPersistentRoute(route);
                    }
                }
            }

            lock (_trades)
            {
                _trades.ForEach(x => x.Tick());
            }

            foreach (var holding in _holdings.Values)
            {
                holding.Consume(MaterialSink);
            }
            
            foreach (var root in _roots.Values)
            {
                var advancement = AdvancementManager.Get(root.Owner);
                foreach (var research in AdvancementManager.Research)
                {
                    advancement.AddResearch(research, root.Extract(research));
                }
            }
        }

        private StellarBodyHolding GetOrCreateHolding(Faction faction, StellarBody stellarBody)
        {
            var holding = GetHolding(faction, stellarBody);
            if (holding == null)
            {
                holding = new StellarBodyHolding(_roots[faction], stellarBody);
                _holdings.Add(CompositeKey<Faction, StellarBody>.Create(faction, stellarBody), holding);
            }
            return holding;
        }
    }
}