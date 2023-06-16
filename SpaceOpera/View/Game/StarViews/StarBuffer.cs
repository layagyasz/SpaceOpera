using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Game.StarViews
{
    public class StarBuffer : GraphicsResource, IRenderable
    {
        private static readonly float s_CycleStep = 0.00002f;

        private readonly VertexPseudo3[] _vertices;
        private readonly VertexPseudo3[] _zBuffer;
        private VertexBuffer<VertexPseudo3>? _buffer;
        private readonly RenderShader _shader;
        private readonly bool _depthTest;

        private bool _dirty;
        private float _cycle;

        public StarBuffer(VertexPseudo3[] vertices, RenderShader shader, bool depthTest)
        {
            _vertices = vertices;
            _zBuffer = new VertexPseudo3[_vertices.Length];
            _buffer = new(PrimitiveType.Points);
            _shader = shader;
            _depthTest = depthTest;
            Dirty();
        }

        public void Dirty()
        {
            _dirty = true;
        }

        public VertexPseudo3 Get(int index)
        {
            return _vertices[index];
        }

        public void Initialize() { }

        public void ResizeContext(Vector3 bounds) { }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            if (_dirty)
            {
                UpdateFromCamera(target.GetModelMatrix() * target.GetViewMatrix());
                _dirty = false;
            }
            _shader.SetFloat("cycle_time", _cycle);
            target.Draw(
                _buffer!, 
                0, 
                _buffer!.Length, 
                new RenderResources(BlendMode.Alpha, _shader)
                { 
                    IsPretransformed = true, 
                    EnableDepthTest = _depthTest 
                });
        }

        public void Update(long delta)
        {
            _cycle = (_cycle + s_CycleStep * delta) % 1;
        }

        protected override void DisposeImpl()
        {
            _buffer?.Dispose();
            _buffer = null;
        }

        private void UpdateFromCamera(Matrix4 camera)
        {
            for (int i=0; i<_vertices.Length; ++i)
            {
                var v = _vertices[i];
                _zBuffer[i] = new((new Vector4(v.Position, 1) * camera).Xyz, v.Color, v.Scale);
            }
            Array.Sort(_zBuffer, (x, y) => x.Position.Z.CompareTo(y.Position.Z));
            _buffer!.Buffer(_zBuffer);
        }
    }
}
