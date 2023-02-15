using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.View.GalaxyViews
{
    public class TransitBuffer : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _vertices;
        private readonly RenderShader _shader;

        public TransitBuffer(VertexBuffer<Vertex3> vertices, RenderShader shader)
        {
            _vertices = vertices;
            _shader = shader;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(_vertices!, 0, _vertices!.Length, new RenderResources(BlendMode.Alpha, _shader));
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _vertices!.Dispose();
            _vertices = null;
        }
    }
}
