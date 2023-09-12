using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.Highlights
{
    public class SubRegionHighlight : RegionHighlight
    {
        public override EventHandler<EventArgs>? Updated { get; set; }

        public override bool Merge => false;
        public override float BorderWidth => 1f;
        public override Color4 BorderColor => new(0.2f, 0.2f, 0.2f, 1f);
        public override Color4 Color => new(0f, 0f, 0f, 0f);

        private SubRegionHighlight() { }

        public static SubRegionHighlight Create()
        {
            return new();
        }

        public override bool Contains(object @object)
        {
            return true;
        }

        public override void Unhook() { }
    }
}
