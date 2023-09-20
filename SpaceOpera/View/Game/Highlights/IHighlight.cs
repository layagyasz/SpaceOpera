using Cardamom.Graphics;

namespace SpaceOpera.View.Game.Highlights
{
    public interface IHighlight
    {
        EventHandler<EventArgs>? Updated { get; set; }

        IRenderable CreateHighlight<TDomain, TRange>(
            HighlightShaders shaders, TDomain domain, IDictionary<TRange, BoundsAndRegionKey> range, float borderWidth)
            where TDomain : notnull where TRange : notnull;
            
        void Unhook();
    }
}
