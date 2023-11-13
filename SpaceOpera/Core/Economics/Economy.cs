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

        private readonly Dictionary<Faction, EconomicFactionHolding> _holdings = new();
        private readonly Dictionary<StellarBody, EconomicZoneRoot> _roots = new();

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
            _holdings.Add(faction, new EconomicFactionHolding(faction));
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

        public EconomicSubzoneHolding CreateSovereignHolding(Faction faction, StellarBodyRegion region)
        {
            var holding = GetOrCreateHolding(faction, region);
            holding.AddStructureNodes((int)region.StructureNodes);
            foreach (var resource in region.Resources)
            {
                holding.AddResourceNodes(Count<ResourceNode>.Create(resource, resource.Size));
            }
            return holding;
        }

        public EconomicZoneRoot? GetRoot(StellarBody stellarBody)
        {
            return _roots.GetValueOrDefault(stellarBody);
        }

        public EconomicFactionHolding GetHolding(Faction faction)
        {
            return _holdings[faction];
        }

        public IEnumerable<PersistentRoute> GetPersistentRoutesFor(Faction faction)
        {
            return _routes.Where(x => x.Faction == faction);
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
            foreach (var holding in _roots.Values)
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

            foreach (var holding in _roots.Values)
            {
                holding.Consume(MaterialSink);
            }
            
            foreach (var root in _holdings.Values)
            {
                var advancement = AdvancementManager.Get(root.Owner);
                foreach (var research in AdvancementManager.Research)
                {
                    advancement.AddResearch(research, root.Extract(research));
                }
            }
        }

        private EconomicZoneHolding GetOrCreateHolding(Faction faction, StellarBody stellarBody)
        {
            var parentHolding = _holdings[faction];
            var holding = parentHolding.GetHolding(stellarBody);
            if (holding == null)
            {
                holding = new EconomicZoneHolding(parentHolding, stellarBody);
                parentHolding.AddHolding(holding);
                GetOrCreateRoot(stellarBody).AddHolding(holding);
            }
            return holding;
        }

        private EconomicSubzoneHolding GetOrCreateHolding(Faction faction, StellarBodyRegion region)
        {
            var parentHolding = GetOrCreateHolding(faction, region.Parent!);
            var holding = parentHolding.GetHolding(region);
            if (holding == null)
            {
                holding = new EconomicSubzoneHolding(parentHolding, region);
                parentHolding.AddHolding(holding);
                GetOrCreateRoot(region).AddHolding(holding);
            }
            return holding;
        }

        private EconomicZoneRoot GetOrCreateRoot(StellarBody stellarBody)
        {
            if (_roots.TryGetValue(stellarBody, out var root))
            {
                return root;
            }
            var newRoot = new EconomicZoneRoot(stellarBody);
            _roots.Add(stellarBody, newRoot);
            return newRoot;
        }

        private EconomicSubzoneRoot GetOrCreateRoot(StellarBodyRegion region)
        {
            var parentRoot = GetOrCreateRoot(region.Parent!);
            var root = parentRoot.GetChild(region);
            if (root == null)
            {
                root = new EconomicSubzoneRoot(region);
                parentRoot.AddChild(root);
            }
            return root;
        }
    }
}