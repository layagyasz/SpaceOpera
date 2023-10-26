using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics
{
    public class EconomicRoot
    {
        public Faction Owner { get; }

        private readonly Inventory _inventory = new(float.MaxValue);

        public EconomicRoot(Faction owner)
        {
            Owner = owner;
        }

        public void Add(IMaterial material, float amount)
        {
            _inventory.MaxAdd(material, amount);
        }

        public float Extract(IMaterial material)
        {
            return _inventory.Extract(material);
        }
    }
}
