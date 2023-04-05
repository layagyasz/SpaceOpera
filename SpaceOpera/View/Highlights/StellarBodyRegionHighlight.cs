using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Highlights
{
    public class StellarBodyRegionHighlight : IHighlight
    {
        public EventHandler<EventArgs>? OnUpdated { get; set; }

        public bool Merge => true;
        public float BorderWidth => 4f;
        public Color4 BorderColor => Color4.Yellow;
        public Color4 Color => new(0, 0, 0, 0);

        public StellarBodyRegion Region { get; }

        public StellarBodyRegionHighlight(StellarBodyRegion region)
        {
            Region = region;
        }

        public bool Contains(object @object)
        {
            if (@object is StellarBodySubRegion region)
            {
                return region.ParentRegion == Region;
            }
            return false;
        }

        public void Unhook() { }
    }
}
