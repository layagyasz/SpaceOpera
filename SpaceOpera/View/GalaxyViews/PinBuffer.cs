using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.View.GalaxyViews
{
    public class PinBuffer : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _vertices;
        private readonly RenderShader _shader;
        private readonly float _width;
        private readonly float _dashLength;

        public PinBuffer(VertexBuffer<Vertex3> vertices, RenderShader shader, float width, float dashLength)
        {
            _vertices = vertices;
            _shader = shader;
            _width = width;
            _dashLength = dashLength;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Draw(RenderTarget target, UiContext context)
        {
            _shader.SetFloat("width", _width);
            _shader.SetFloat("dash_length", _dashLength);
            target.Draw(_vertices!, 0, _vertices!.Length, new RenderResources(BlendMode.None, _shader));
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _vertices!.Dispose();
            _vertices = null;
        }
    }
}
