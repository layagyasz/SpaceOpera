using OpenTK.Mathematics;

namespace SpaceOpera.View.Highlights
{
    public interface IHighlight
    {
        bool Dirty { get; set; }
        bool Merge { get; }
        float BorderWidth { get; }
        Color4 BorderColor { get; }
        Color4 Color { get; }
        bool Contains(object @object);
        void Unhook();
    }
}
