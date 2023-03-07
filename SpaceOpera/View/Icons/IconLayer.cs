using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class IconLayer : IRenderable
    {
        public class Definition
        {
            public Vertex3[] Vertices { get; set; }
            public Color4 Color { get; set; }
            public string Texture { get; set; }

            public Definition(Color4 color, string texture)
            {
                Vertices = Utils.CreateRect(new(new(), new(1, 1)), color, new(new(), new(1, 1)));
                Color = color;
                Texture = texture;
            }

            public Definition(Vertex3[] vertices, Color4 color, string texture)
            {
                Vertices = vertices;
                Color = color;
                Texture = texture;
            }

            public IconLayer Create(RenderShader shader, UiElementFactory uiElementFactory)
            {
                var tex = uiElementFactory.GetTexture(Texture);
                var vertices = new Vertex3[Vertices.Length];
                for (int i=0; i<vertices.Length; ++i)
                {
                    vertices[i] =
                        new(
                            Vertices[i].Position, 
                            Vertices[i].Color, 
                            tex.TextureView.Min + Vertices[i].TexCoords * tex.TextureView.Size);
                }
                return new IconLayer(vertices, tex.Texture!, shader);
            }

            public Definition WithColor(Color4 color)
            {
                return new(Vertices, color, Texture);
            }
        }

        private readonly Vector2[] _points;
        private readonly Vertex3[] _vertices;
        private readonly Texture _texture;
        private readonly RenderShader _shader;

        private IconLayer(Vertex3[] vertices, Texture texture, RenderShader shader)
        {
            _points = vertices.Select(x => x.Position.Xy).ToArray();
            _vertices = vertices;
            _texture = texture;
            _shader = shader;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(
                _vertices, PrimitiveType.Triangles, 0, _vertices.Length, new(BlendMode.Alpha, _shader, _texture));
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void SetAlpha(float alpha)
        {
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Color.A = alpha;
            }
        }

        public void SetSize(Vector2 size)
        {
            for (int i = 0; i < _points.Length; ++i)
            {
                _vertices[i].Position = new(size * _points[i]);
            }
        }

        public void Update(long delta) { }
    }
}
