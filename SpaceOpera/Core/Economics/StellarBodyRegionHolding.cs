using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics
{
    class StellarBodyRegionHolding : EconomicSubzone
    {
        public StellarBodyRegion Region { get; }

        public StellarBodyRegionHolding(StellarBodyHolding parent, StellarBodyRegion region)
            : base(parent)
        {
            Region = region;
        }
    }
}