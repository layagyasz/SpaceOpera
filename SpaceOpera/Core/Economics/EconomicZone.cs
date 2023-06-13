using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics
{
    public abstract class EconomicZone : ProjectHub, ITickable
    {
        public abstract string Name { get; }
        public Faction Owner { get; }
        public FactionAdvancementManager AdvancementManager { get; }
        public uint Population { get; protected set; }

        private readonly Dictionary<object, EconomicSubzone> _subzones = new();
        private readonly Inventory _inventory = new(float.MaxValue);
        private readonly MultiCount<Recipe> _production = new ();

        protected EconomicZone(Faction owner, FactionAdvancementManager advancementManager)
        {
            Owner = owner;
            AdvancementManager = advancementManager;
        }

        public void AddSubzone(object key, EconomicSubzone subzone)
        {
            _subzones.Add(key, subzone);
        }

        public void AdjustProduction(MultiCount<Recipe> allocation)
        {
            _production.Add(allocation);
        }

        public EconomicSubzone? GetSubzone(object key)
        {
            _subzones.TryGetValue(key, out var result);
            return result;
        }

        public IEnumerable<EconomicSubzone> GetSubzones()
        {
            return _subzones.Values;
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
                            AdvancementManager.AddResearch(transform.Key, total);
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
                Spend(sink.Materials, Population);
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