using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public record class IconLayer(Vector2[]? Vertices, Color4 Color, string Texture, bool IsInfo)
    {
        public IconLayer WithColor(Color4 color)
        {
            return new(Vertices, color, Texture, IsInfo);
        }

        public IconLayer WithInfo(bool isInfo)
        {
            return new(Vertices, Color, Texture, isInfo);
        }
    }
}
