using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Scenes.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemRig : GraphicsResource, IRenderable
    {
        private readonly Orbit _orbit;
        private StarSubSystemView? _view;
        private readonly float _radius;

        public StarSubSystemRig(Orbit orbit, StarSubSystemView view, float radius)
        {
            _orbit = orbit;
            _view = view;
            _radius = radius;
        }

        protected override void DisposeImpl()
        {
            _view!.Dispose();
            _view = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushModelMatrix(Matrix4.CreateTranslation(_radius, 0, 0));
            _view!.Draw(target, context);
            target.PopModelMatrix();
        }

        public void Initialize()
        {
            _view!.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _view!.ResizeContext(bounds);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            _view!.SetHighlight(layer, highlight);
        }

        public void Update(long delta)
        {
            _view!.Update(delta);
        }
    }
}
