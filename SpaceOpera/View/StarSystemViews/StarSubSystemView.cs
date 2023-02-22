using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemView : GraphicsResource, IRenderable
    {
        private HighlightLayer<INavigable>? _highlightLayer;
        private PinBuffer? _pinBuffer;

        public StarSubSystemView(HighlightLayer<INavigable> highlightLayer, PinBuffer buffer)
        {
            _highlightLayer = highlightLayer;
            _pinBuffer = buffer;
        }

        protected override void DisposeImpl()
        {
            _highlightLayer!.Dispose();
            _highlightLayer = null;
            _pinBuffer!.Dispose();
            _pinBuffer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            _pinBuffer!.Draw(target, context);
            _highlightLayer!.Draw(target, context);
        }

        public void Initialize()
        {
            _highlightLayer!.Initialize();
            _pinBuffer!.Initialize();
        }

        public void ResizeContext(Vector3 bounds) { }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            _highlightLayer!.SetLayer(layer, highlight);
        }

        public void Update(long delta) { }
    }
}
