using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Fronts
{
    public struct FrontEdge
    {
        public StellarBodySubRegion Region { get; }
        public StellarBodySubRegion Neighbor { get; }
    }
}
