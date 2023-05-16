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
        private readonly MultiQuantity<IMaterial> _inventory = new();
        private readonly MultiCount<Recipe> _production = new ();

        protected EconomicZone(Faction owner, FactionAdvancementManager advancementManager)
        {
            Owner = owner;
            AdvancementManager = advancementManager;
        }

        public void Add(MultiQuantity<IMaterial> materials)
        {
            _inventory.Add(materials);
        }

        public void AddSubzone(object key, EconomicSubzone subzone)
        {
            _subzones.Add(key, subzone);
        }

        public void AdjustProduction(MultiCount<Recipe> allocation)
        {
            _production.Add(allocation);
        }


        public bool Contains(MultiQuantity<IMaterial> materials)
        {
            foreach (var material in materials)
            {
                if (_inventory[material.Key] < material.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public float GetInventoryQuantity(IMaterial material)
        {
            return _inventory[material];
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

        public MultiQuantity<IMaterial> GetInventory()
        {
            return _inventory.Copy();
        }

        public void Remove(MultiQuantity<IMaterial> materials)
        {
            foreach (var material in materials)
            {
                _inventory.Add(material.Key, -material.Value);
            }
        }

        public float Spend(MultiQuantity<IMaterial> unitCost, float maxUnits)
        {
            foreach (var cost in unitCost)
            {
                maxUnits = Math.Min(_inventory.Get(cost.Key) / cost.Value, maxUnits);
            }
            foreach (var cost in unitCost)
            {
                _inventory.Add(cost.Key, maxUnits * cost.Value);
            }
            return maxUnits;
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
                            _inventory.Add(transform.Key, total);
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
    }
}