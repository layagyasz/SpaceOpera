using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class TextureLayer : IIconLayer
    {
        public class Definition : IIconLayerDefinition
        {
            public Color4 Color { get; set; }
            public string Texture { get; set; }

            public Definition(Color4 color, string texture)
            {
                Color = color;
                Texture = texture;
            }

            public IIconLayer Create(IconShaders shaders, UiElementFactory uiElementFactory)
            {
                var tex = uiElementFactory.GetTexture(Texture);
                return new TextureLayer(
                    Utils.CreateRect(new(), Color, tex.TextureView), tex.Texture!, shaders.Texture);
            }
        }

        private readonly Vertex3[] _vertices;
        private readonly Texture _texture;
        private readonly RenderShader _shader;

        private TextureLayer(Vertex3[] vertices, Texture texture, RenderShader shader)
        {
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
            for (int i=0; i<_vertices.Length; ++i)
            {
                _vertices[i].Color.A = alpha;
            }
        }

        public void SetSize(Vector2 size)
        {
            _vertices[1].Position = new(size.X, 0, 0);
            _vertices[2].Position = new(0, size.Y, 0);
            _vertices[3].Position = new(size.X, 0, 0);
            _vertices[4].Position = new(0, size.Y, 0);
            _vertices[5].Position = new(size.X, size.Y, 0);
        }

        public void Update(long delta) { }
    }
}
