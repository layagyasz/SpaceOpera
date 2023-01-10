using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class EconomicZone : ProjectHub, ITickable
    {
        public Faction Owner { get; }
        public uint Population { get; protected set; }

        private readonly Dictionary<object, EconomicSubzone> _Subzones = new Dictionary<object, EconomicSubzone>();
        private readonly MultiQuantity<IMaterial> _Inventory = new MultiQuantity<IMaterial>();
        private readonly MultiCount<ActualizedRecipe> _Production = new MultiCount<ActualizedRecipe>();

        protected EconomicZone(Faction Owner)
        {
            this.Owner = Owner;
        }

        public void Add(MultiQuantity<IMaterial> Materials)
        {
            _Inventory.Add(Materials);
        }

        public void AddSubzone(object Key, EconomicSubzone Subzone)
        {
            _Subzones.Add(Key, Subzone);
        }

        public void AdjustProduction(MultiCount<ActualizedRecipe> Allocation)
        {
            _Production.Add(Allocation);
        }


        public bool Contains(MultiQuantity<IMaterial> Materials)
        {
            foreach (var material in Materials)
            {
                if (_Inventory[material.Key] < material.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public float GetInventoryQuantity(IMaterial Material)
        {
            return _Inventory[Material];
        }

        public EconomicSubzone GetSubzone(object Key)
        {
            _Subzones.TryGetValue(Key, out EconomicSubzone subzone);
            return subzone;
        }

        public IEnumerable<EconomicSubzone> GetSubzones()
        {
            return _Subzones.Values;
        }

        public MultiQuantity<IMaterial> GetInventory()
        {
            return _Inventory.Copy();
        }

        public void Remove(MultiQuantity<IMaterial> Materials)
        {
            foreach (var material in Materials)
            {
                _Inventory.Add(material.Key, -material.Value);
            }
        }

        public float Spend(MultiQuantity<IMaterial> UnitCost, float MaxUnits)
        {
            foreach (var cost in UnitCost)
            {
                MaxUnits = Math.Min(_Inventory.Get(cost.Key) / cost.Value, MaxUnits);
            }
            foreach (var cost in UnitCost)
            {
                _Inventory.Add(cost.Key, MaxUnits * cost.Value);
            }
            return MaxUnits;
        }

        public void Tick()
        {
            foreach (var production in _Production)
            {
                foreach (var transform in production.Key.Transformation)
                {
                    var total = transform.Value * production.Value * production.Key.BaseRecipe.Structure.MaxWorkers;
                    switch (transform.Key.Type)
                    {
                        case MaterialType.MATERIAL_CONTINUOUS:
                            _Inventory.Add(transform.Key, total);
                            break;
                        case MaterialType.RESEARCH:
                            Owner.AddResearch(transform.Key, total);
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