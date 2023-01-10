using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class StellarBodyRegionHolding : EconomicSubzone
    {
        public StellarBodyRegion Region { get; }

        public StellarBodyRegionHolding(StellarBodyHolding Parent, StellarBodyRegion Region)
            : base(Parent)
        {
            this.Region = Region;
        }
    }
}