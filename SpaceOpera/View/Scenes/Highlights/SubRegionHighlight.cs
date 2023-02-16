using OpenTK.Mathematics;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class SubRegionHighlight : IHighlight
    {
        public EventHandler<EventArgs>? OnUpdated { get; set; }

        public bool Merge => false;
        public float BorderWidth => 1f;
        public Color4 BorderColor => new(0.4f, 0.4f, 0.4f, 1f);
        public Color4 Color => new(0f, 0f, 0f, 0f);

        public bool Contains(object @object)
        {
            return true;
        }

        public void Unhook() { }
    }
}
