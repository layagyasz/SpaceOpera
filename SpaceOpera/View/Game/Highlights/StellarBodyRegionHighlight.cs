using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Game.Highlights
{
    public class StellarBodyRegionHighlight : RegionHighlight
    {
        public override EventHandler<EventArgs>? Updated { get; set; }

        public override bool Merge => true;
        public override float BorderWidth => 4f;
        public override Color4 BorderColor => Color4.Yellow;
        public override Color4 Color => Color4.Yellow;

        public StellarBodyRegion Region { get; }

        public StellarBodyRegionHighlight(StellarBodyRegion region)
        {
            Region = region;
        }

        public override bool Contains(object @object)
        {
            if (@object is StellarBodySubRegion region)
            {
                return region.ParentRegion == Region;
            }
            return false;
        }

        public override void Unhook() { }
    }
}
