using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    public class StellarBodyHolding : EconomicZone
    {
        public override string Name => StellarBody.Name;
        public StellarBody StellarBody { get; }

        private readonly List<Division> _divisions = new();

        public StellarBodyHolding(EconomicRoot root, StellarBody stellarBody)
            : base(root)
        {
            StellarBody = stellarBody;
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