using OpenTK.Mathematics;

namespace SpaceOpera.View.Scenes.Highlights
{
    public interface IHighlight
    {
        EventHandler<EventArgs>? OnUpdated { get; set; }
        Color4 BorderColor { get; }
        Color4 Color { get; }
        bool Contains(object @object);
        void Unhook();
    }
}
