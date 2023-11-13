using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class EconomicFactionHolding
    {
        public Faction Owner { get; }

        private readonly Inventory _inventory = new(float.MaxValue);
        private readonly List<EconomicZoneHolding> _holdings = new();

        public EconomicFactionHolding(Faction owner)
        {
            Owner = owner;
        }

        public void Add(IMaterial material, float amount)
        {
            _inventory.MaxAdd(material, amount);
        }

        public void AddHolding(EconomicZoneHolding holding)
        {
            _holdings.Add(holding);
        }

        public float Extract(IMaterial material)
        {
            return _inventory.Extract(material);
        }

        public EconomicZoneHolding? GetHolding(StellarBody stellarBody)
        {
            return _holdings.FirstOrDefault(x => x.StellarBody == stellarBody);
        }

        public IEnumerable<EconomicZoneHolding> GetHoldings()
        {
            return _holdings;
        }
    }
}
