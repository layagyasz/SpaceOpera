using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class PatternLayer : IIconLayer
    {
        public class Definition : IIconLayerDefinition
        {
            public Color4 Color { get; set; }
            public Vector2[] Vertices { get; set; }

            public Definition(Color4 color, Vector2[] vertices)
            {
                Color = color;
                Vertices = vertices;
            }

            public IIconLayer Create(IconShaders shaders, UiElementFactory uiElementFactory)
            {
                return new PatternLayer(Vertices, shaders.NoTexture);
            }
        }

        private readonly Vector2[] _points;
        private readonly Vertex3[] _vertices;
        private readonly RenderShader _shader;

        private PatternLayer(Vector2[] points, RenderShader shader)
        {
            _points = points;
            _vertices = new Vertex3[points.Length];
            _shader = shader;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(
                _vertices, PrimitiveType.Triangles, 0, _vertices.Length, new(BlendMode.Alpha, _shader));
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
            for (int i=0; i< _points.Length; ++i)
            {
                _vertices[i].Position = new(size * _points[i]);
            }
        }

        public void Update(long delta) { }
    }
}
