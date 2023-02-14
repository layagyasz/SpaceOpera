using Cardamom.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View
{
    public struct VertexPseudo3
    {
        [VertexAttribute(0, 3, VertexAttribPointerType.Float, false)]
        public Vector3 Position;

        [VertexAttribute(1, 4, VertexAttribPointerType.Float, false)]
        public Color4 Color;

        [VertexAttribute(2, 2, VertexAttribPointerType.Float, false)]
        public Vector2 Scale;

        public VertexPseudo3(Vector3 position, Color4 color, Vector2 scale)
        {
            Position = position;
            Color = color;
            Scale = scale;
        }
    }
}
