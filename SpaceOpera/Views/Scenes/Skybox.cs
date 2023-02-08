using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace SpaceOpera.Views.Scenes
{
    public class Skybox : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _buffer;
        private Texture? _texture;
        private readonly RenderShader _shader;

        public Skybox(VertexBuffer<Vertex3> buffer, Texture texture, RenderShader shader)
        {
            _buffer = buffer;
            _texture = texture;
            _shader = shader;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.Draw(_buffer!, 0, _buffer!.Length, new RenderResources(BlendMode.None, _shader, _texture!));
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _buffer!.Dispose();
            _buffer = null;
            _texture!.Dispose();
            _texture = null;
        }
    }
}
