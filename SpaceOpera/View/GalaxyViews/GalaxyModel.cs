using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.View.Common;
using SpaceOpera.View.StarViews;

namespace SpaceOpera.View.GalaxyViews
{
    public class GalaxyModel : GraphicsResource, IRenderable
    {
        private StarBuffer? _stars;
        private TransitBuffer? _transits;
        private PinBuffer? _pins;

        public GalaxyModel(StarBuffer stars, TransitBuffer transits, PinBuffer pinBuffer)
        {
            _stars = stars;
            _transits = transits;
            _pins = pinBuffer;
        }

        public void Dirty()
        {
            _stars!.Dirty();
        }

        public void Initialize()
        {
            _stars!.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _stars!.ResizeContext(bounds);
        }
        
        public void Draw(RenderTarget target, UiContext context)
        {
            _transits!.Draw(target, context);
            _pins!.Draw(target, context);
            _stars!.Draw(target, context);
        }

        public void Update(long delta)
        {
            _stars!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _stars!.Dispose();
            _stars = null;
            _transits!.Dispose();
            _transits = null;
            _pins!.Dispose();
            _pins = null;
        }
    }
}
