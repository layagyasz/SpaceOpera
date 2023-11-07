using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace SpaceOpera.View.Components
{
    public class ColorSwatch : ClassedUiElement
    {
        private readonly Vertex3[] _vertices;

        private Vector2 _size;
        private RenderShader? _shader;
        private UniformBuffer? _uniforms;

        public ColorSwatch(Class @class, IElementController controller, Color4 color)
            : base(@class, controller)
        {
            _vertices = new Vertex3[6];
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i] = new(new(Utils.UnitTriangles[i]), color, new());
            }
            SetAttributes(@class.Get(Class.State.None));
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            if (Visible)
            {
                target.PushTranslation(Position + LeftMargin);
                context.Register(this);
                _uniforms!.Bind(0);
                _shader!.SetVector2("size", _size);
                target.Draw(
                    _vertices, PrimitiveType.Triangles, 0, _vertices.Length, new(BlendMode.Alpha, _shader!));
                target.PopModelMatrix();
            }
        }

        public override float? GetRayIntersection(Ray3 ray)
        {
            if (ray.Point.X >= 0
                && ray.Point.Y >= 0
                && ray.Point.X <= _vertices[5].Position.X
                && ray.Point.Y <= _vertices[5].Position.Y)
            {
                return ray.Point.Z / ray.Direction.Z;
            }
            return null;
        }

        public void SetAlpha(float alpha)
        {
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Color.A = alpha;
            }
        }

        public void SetColor(Color4 color)
        {
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Color.R = color.R;
                _vertices[i].Color.G = color.G;
                _vertices[i].Color.B = color.B;
            }
        }

        public override void SetAttributes(ClassAttributes attributes)
        {
            base.SetAttributes(attributes);
            SetAlpha(attributes.BackgroundColor[0].A);
            SetDyamicSizeImpl(TrueSize.Xy);
            _shader = attributes.BackgroundShader;
            _uniforms = attributes.GetUniforms();
        }

        public override void Update(long delta) { }

        protected override void DisposeImpl() { }

        protected override void SetDyamicSizeImpl(Vector2 size)
        {
            _size = size;
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Position = new(size * Utils.UnitTriangles[i]);
            }
        }
    }
}
