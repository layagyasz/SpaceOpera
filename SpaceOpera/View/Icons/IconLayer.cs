using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class IconLayer
    {
        public Vector2[] Vertices { get; set; }
        public Color4 Color { get; set; }
        public string Texture { get; set; }

        public IconLayer(Color4 color, string texture)
        {
            Vertices = Utils.UnitTriangles;
            Color = color;
            Texture = texture;
        }

        public IconLayer(Vector2[] vertices, Color4 color, string texture)
        {
            Vertices = vertices;
            Color = color;
            Texture = texture;
        }

        public IconLayer WithColor(Color4 color)
        {
            return new(Vertices, color, Texture);
        }
    }
}
