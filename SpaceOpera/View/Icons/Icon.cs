using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class Icon : ClassedUiElement
    {
        private readonly Vertex3[] _vertices;
        private Texture? _texture;
        private readonly RenderShader _shader;

        public Icon(
            Class @class, IElementController controller, Texture texture, RenderShader shader, float resolution)
            : base(@class, controller)
        {
            _vertices = new Vertex3[6];
            for (int i=0; i<_vertices.Length; ++i)
            {
                _vertices[i] = new(new(Utils.UnitTriangles[i]), Color4.White, resolution * Utils.UnitTriangles[i]);
            }
            _texture = texture;
            _shader = shader;
            SetAttributes(@class.Get(Class.State.None));
        }

        public override void Draw(RenderTarget target, UiContext context)
        {
            if (Visible)
            {
                target.PushTranslation(Position + LeftMargin);
                context.Register(this);
                target.Draw(
                    _vertices, PrimitiveType.Triangles, 0, _vertices.Length, new(BlendMode.Alpha, _shader, _texture!));
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

        public override void SetAttributes(ClassAttributes attributes)
        {
            base.SetAttributes(attributes);
            SetAlpha(attributes.BackgroundColor[0].A);
            SetDyamicSizeImpl(TrueSize.Xy);
        }

        public override void Update(long delta) { }

        protected override void DisposeImpl()
        {
            _texture!.Dispose();
            _texture = null;
        }

        protected override void SetDyamicSizeImpl(Vector2 size)
        {
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Position = new(size * Utils.UnitTriangles[i]);
            }
        }

        public void SetAlpha(float alpha)
        {
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i].Color.A = alpha;
            }
        }
    }
}
