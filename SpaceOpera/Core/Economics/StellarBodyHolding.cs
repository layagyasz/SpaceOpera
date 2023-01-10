using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class StellarBodyHolding : EconomicZone
    {
        public StellarBody StellarBody { get; }

        private readonly List<Division> _Divisions = new List<Division>();

        public StellarBodyHolding(Faction Owner, StellarBody StellarBody)
            : base(Owner)
        {
            this.StellarBody = StellarBody;
        }

        public List<Division> GetDivisions()
        {
            return _Divisions.ToList();
        }

        public void AddDivision(Division Division)
        {
            _Divisions.Add(Division);
        }

        public void RemoveDivision(Division Division)
        {
            _Divisions.Remove(Division);
        }
    }
}