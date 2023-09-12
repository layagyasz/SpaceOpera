using SpaceOpera.View.Game.Common;

namespace SpaceOpera.View.Game.Highlights
{
    public readonly struct BoundsAndRegionKey
    {
        public object RegionKey { get; }
        public SpaceSubRegionBounds Bounds { get; }

        public BoundsAndRegionKey(object regionKey, SpaceSubRegionBounds bounds)
        {
            RegionKey = regionKey;
            Bounds = bounds;
        }
    }
}
