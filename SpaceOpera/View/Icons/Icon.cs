using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace SpaceOpera.View.Icons
{
    public class Icon : ClassedUiElement
    {
        private Vector2 _size;
        private readonly List<IIconLayer> _layers;

        public Icon(Class @class, IElementController controller, IEnumerable<IIconLayer> layers)
            : base(@class, controller)
        {
            _layers = layers.ToList();
        }

        public override void Draw(RenderTarget target, UiContext context)
        {
            if (Visible)
            {
                target.PushTranslation(Position);
                _layers.ForEach(x => x.Draw(target, context));
                target.PopModelMatrix();
            }
        }

        public override float? GetRayIntersection(Ray3 ray)
        {
            if (ray.Point.X >= 0 && ray.Point.Y >= 0 && ray.Point.X <= _size.X && ray.Point.Y <= _size.Y)
            {
                return ray.Point.Z / ray.Direction.Z;
            }
            return null;
        }

        public override void SetAttributes(ClassAttributes attributes)
        {
            base.SetAttributes(attributes);
            SetAlpha(attributes.BackgroundColor[0].A);
        }

        public override void Update(long delta) { }

        protected override void DisposeImpl() { }

        protected override void SetDyamicSizeImpl(Vector2 size)
        {
            _size = size;
            _layers.ForEach(x => x.SetSize(size));
        }

        private void SetAlpha(float alpha)
        {
            _layers.ForEach(x => x.SetAlpha(alpha));
        }
    }
}
