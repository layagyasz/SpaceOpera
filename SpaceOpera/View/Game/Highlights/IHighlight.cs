using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.Highlights
{
    public interface IHighlight
    {
        EventHandler<EventArgs>? Updated { get; set; }
        bool Merge { get; }
        float BorderWidth { get; }
        Color4 BorderColor { get; }
        Color4 Color { get; }
        bool Contains(object @object);
        void Unhook();
    }
}
