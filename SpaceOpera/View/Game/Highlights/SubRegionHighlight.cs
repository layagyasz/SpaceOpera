using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.Highlights
{
    public class SubRegionHighlight : IHighlight
    {
        public EventHandler<EventArgs>? Updated { get; set; }

        public bool Merge => false;
        public float BorderWidth => 1f;
        public Color4 BorderColor => new(0.2f, 0.2f, 0.2f, 1f);
        public Color4 Color => new(0f, 0f, 0f, 0f);

        private SubRegionHighlight() { }

        public static SubRegionHighlight Create()
        {
            return new();
        }

        public bool Contains(object @object)
        {
            return true;
        }

        public void Unhook() { }
    }
}
