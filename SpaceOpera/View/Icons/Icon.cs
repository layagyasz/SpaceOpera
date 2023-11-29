using Cardamom;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Utils.Suppliers.Promises;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class Icon : ClassedUiElement
    {
        public CompositeKey<object, IconResolution> Key { get; }

        private readonly IPromise<IconImage> _image;
        private readonly RenderShader _shader;
        private readonly IIconDisposer _disposer;

        private readonly Vertex3[] _vertices;
        private Texture? _texture;

        public Icon(
            CompositeKey<object, IconResolution> key, 
            Class @class, 
            IElementController controller,
            IPromise<IconImage> image,
            RenderShader shader,
            IIconDisposer disposer)
            : base(@class, controller)
        {
            Key = key;
            _image = image;
            _vertices = new Vertex3[6];
            _shader = shader;
            _disposer = disposer;
            SetAttributes(@class.Get(Class.State.None));
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            if (_texture == null && _image.HasValue())
            {
                HandleRasterized();
            }
            if (Visible && _texture != null)
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

        public Texture GetTexture()
        {
            return _texture!;
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
            // Make sure rasterization finishes before disposing.
            _image.Get();
            _disposer.Dispose(this);
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

        private void HandleRasterized()
        {
            var image = _image.Get();
            for (int i = 0; i < _vertices.Length; ++i)
            {
                _vertices[i] =
                    new(
                        _vertices[i].Position,
                        image.Color,
                        image.TextureView.Min + image.TextureView.Size * Utils.UnitTriangles[i]);
            }
            _texture = image.Texture;
        }
    }
}
