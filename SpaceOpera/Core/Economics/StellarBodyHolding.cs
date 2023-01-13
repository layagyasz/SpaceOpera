using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    class StellarBodyHolding : EconomicZone
    {
        public StellarBody StellarBody { get; }

        private readonly List<Division> _divisions = new();

        public StellarBodyHolding(Faction owner, StellarBody stellarBody)
            : base(owner)
        {
            this.StellarBody = stellarBody;
        }

        public List<Division> GetDivisions()
        {
            return _divisions.ToList();
        }

        public void AddDivision(Division division)
        {
            _divisions.Add(division);
        }

        public void RemoveDivision(Division division)
        {
            _divisions.Remove(division);
        }
    }
}