using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemView : GraphicsResource, IRenderable
    {
        private StellarBodyModel? _model;
        private HighlightLayer<INavigable, INavigable>? _highlightLayer;
        private PinBuffer? _pinBuffer;

        public StarSubSystemView(
            StellarBodyModel model, HighlightLayer<INavigable, INavigable> highlightLayer, PinBuffer buffer)
        {
            _model = model;
            _highlightLayer = highlightLayer;
            _pinBuffer = buffer;
        }

        protected override void DisposeImpl()
        {
            _model!.Dispose();
            _model = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
            _pinBuffer!.Dispose();
            _pinBuffer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            _model!.Draw(target, context);
            _pinBuffer!.Draw(target, context);
            _highlightLayer!.Draw(target, context);
        }

        public void Initialize()
        {
            _highlightLayer!.Initialize();
            _pinBuffer!.Initialize();
        }

        public void ResizeContext(Vector3 bounds) { }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight)
        {
            _highlightLayer!.SetLayer(layer, highlight);
        }

        public void Update(long delta) { }
    }
}
