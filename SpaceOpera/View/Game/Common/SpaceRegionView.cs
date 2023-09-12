using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.Common
{
    public class SpaceRegionView : GraphicsResource, IRenderable
    {
        private VertexBuffer<Vertex3>? _outline;
        private readonly RenderShader _outlineShader;
        private VertexBuffer<Vertex3>? _fill;
        private readonly RenderShader _fillShader;

        public SpaceRegionView(
            VertexBuffer<Vertex3> outline,
            RenderShader outlineShader,
            VertexBuffer<Vertex3> fill,
            RenderShader fillShader)
        {
            _outline = outline;
            _outlineShader = outlineShader;
            _fill = fill;
            _fillShader = fillShader;
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 context) { }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.Draw(_fill!, 0, _fill!.Length, new(BlendMode.Alpha, _fillShader) { EnableDepthMask = false });
            target.Draw(
                _outline!, 0, _outline!.Length, new(BlendMode.Alpha, _outlineShader) { EnableDepthMask = false });
        }

        public void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _outline!.Dispose();
            _outline = null;
            _fill!.Dispose();
            _fill = null;
        }
    }
}
