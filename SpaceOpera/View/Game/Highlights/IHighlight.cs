using Cardamom.Graphics;

namespace SpaceOpera.View.Game.Highlights
{
    public interface IHighlight
    {
        EventHandler<EventArgs>? Updated { get; set; }

        IRenderable CreateHighlight<TRange>(
            HighlightShaders shaders, IEnumerable<BoundsAndRegionKey> range, float borderWidth);

        void Unhook();
    }
}
