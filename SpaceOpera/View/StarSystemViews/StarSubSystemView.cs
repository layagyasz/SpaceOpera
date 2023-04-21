using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.FormationViews;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemView : GraphicsResource, IRenderable
    {
        private StellarBodyModel? _model;
        private HighlightLayer<INavigable, INavigable>? _highlightLayer;
        private FormationSubLayer<object>? _formationLayer;
        private PinBuffer? _pinBuffer;

        public StarSubSystemView(
            StellarBodyModel model,
            HighlightLayer<INavigable, INavigable> highlightLayer,
            FormationSubLayer<object> formationLayer,
            PinBuffer buffer)
        {
            _model = model;
            _highlightLayer = highlightLayer;
            _formationLayer = formationLayer;
            _pinBuffer = buffer;
        }

        protected override void DisposeImpl()
        {
            _model!.Dispose();
            _model = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
            _formationLayer = null;
            _pinBuffer!.Dispose();
            _pinBuffer = null;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _model!.Draw(target, context);
            _pinBuffer!.Draw(target, context);
            _highlightLayer!.Draw(target, context);
            _formationLayer!.Dirty();
            _formationLayer!.UpdateFromCamera(target, context);
        }

        public FormationSubLayer<object> GetFormationSubLayer()
        {
            return _formationLayer!;
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

        public void Update(long delta)
        {
            _highlightLayer!.Update(delta);
        }
    }
}
