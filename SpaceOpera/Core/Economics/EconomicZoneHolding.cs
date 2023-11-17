using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicZoneHolding : IProjectHub, ITickable
    {
        public string Name => StellarBody.Name;
        public Faction Owner => Parent.Owner;
        public EconomicFactionHolding Parent { get; }
        public StellarBody StellarBody { get; }

        private readonly Dictionary<StellarBodyRegion, EconomicSubzoneHolding> _holdings = new();
        private readonly Inventory _inventory = new(float.MaxValue);
        private readonly MultiCount<Recipe> _production = new ();

        public EconomicZoneHolding(EconomicFactionHolding parent, StellarBody stellarBody)
        {
            Parent = parent;
            StellarBody = stellarBody;
        }

        public void AddHolding(EconomicSubzoneHolding holding)
        {
            _holdings.Add(holding.Region, holding);
        }

        public void AdjustProduction(MultiCount<Recipe> allocation)
        {
            _production.Add(allocation);
        }

        public EconomicSubzoneHolding? GetHolding(StellarBodyRegion region)
        {
            _holdings.TryGetValue(region, out var result);
            return result;
        }

        public IEnumerable<EconomicSubzoneHolding> GetHoldings()
        {
            return _holdings.Values;
        }

        public long GetPopulation()
        {
            return _holdings.Values.Sum(x => x.GetPopulation());
        }

        public IEnumerable<IProject> GetProjects()
        {
            return _holdings.Values.SelectMany(x => x.GetProjects());
        }

        public int GetRegionCount(bool isTraversable)
        {
            return _holdings.Values.Sum(x => x.GetRegionCount(isTraversable));
        }

        public void Return(MultiQuantity<IMaterial> materials)
        {
            _inventory.TryAdd(materials);
        }

        public float Spend(MultiQuantity<IMaterial> unitCost, float maxUnits)
        {
            return _inventory.MaxSpend(unitCost, maxUnits);
        }

        public void Tick()
        {
            foreach (var production in _production)
            {
                foreach (var transform in production.Key.Transformation)
                {
                    var total = transform.Value * production.Value * production.Key.Structure!.MaxWorkers;
                    switch (transform.Key.Type)
                    {
                        case MaterialType.MaterialContinuous:
                            _inventory.MaxAdd(transform.Key, total);
                            break;
                        case MaterialType.Research:
                            Parent.Add(transform.Key, total);
                            break;
                        default:
                            throw new InvalidOperationException(
                                string.Format("Unsupported MaterialType: {0}", transform.Key.Type));
                    }
                }
            }
        }

        public void Consume(MaterialSink MaterialSink)
        {
            foreach (var sink in MaterialSink.PopulationSink)
            {
                Spend(sink.Materials, GetPopulation());
            }
        }

        public Inventory.ChangeStatus Load(Inventory inventory, MultiQuantity<IMaterial> materials)
        {
            return inventory.MaxTransferFrom(_inventory, materials, float.MaxValue);
        }

        public Inventory.ChangeStatus Unload(Inventory inventory)
        {
            return inventory.MaxTransferTo(_inventory, float.MaxValue);
        }
    }
}