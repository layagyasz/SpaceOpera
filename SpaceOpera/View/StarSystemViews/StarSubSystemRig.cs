using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemRig : GraphicsResource, IRenderable, IControlledElement
    {
        private static readonly float s_GameYearInMillis = 360000f;
        private static readonly int s_OrbitAccuracy = 20;
        private static readonly float s_OrbitPrecision = 0.01f;

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }

        private readonly StellarBody _stellarBody;
        private readonly StarCalendar _calendar;
        private StarSubSystemView? _view;
        private readonly SubRegionInteractor[] _interactors;
        private readonly float _scale;

        private readonly float _offset;
        private readonly float _step;

        public StarSubSystemRig(
            IElementController controller,
            StellarBody stellarBody, 
            StarCalendar calendar, 
            StarSubSystemView view, 
            SubRegionInteractor[] interactors, 
            float scale)
        {
            Controller = controller;
            _stellarBody = stellarBody;
            _calendar = calendar;
            _view = view;
            _interactors = interactors;
            foreach (var interactor in _interactors)
            {
                interactor.Parent = this;
            }
            _scale = scale;
            _offset = MathHelper.TwoPi * stellarBody.Orbit.TimeOffset;
            _step = -MathHelper.TwoPi * 0.001f * s_GameYearInMillis / stellarBody.GetYearLengthInMillis();
        }

        protected override void DisposeImpl()
        {
            _view!.Dispose();
            _view = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            var positionPolar = 
                _stellarBody.GetSolarOrbitPosition(
                    _stellarBody.GetSolarOrbitProgression(
                        _offset + _step * _calendar.GetMillis(), s_OrbitPrecision, s_OrbitAccuracy));
            positionPolar.Radius = _scale * MathF.Log(positionPolar.Radius + 1);
            var positionCartesian = positionPolar.AsCartesian();
            target.PushModelMatrix(Matrix4.CreateTranslation(positionCartesian.X, 0, positionCartesian.Y));
            foreach (var interactor in _interactors)
            {
                context.Register(interactor);
            }
            _view!.Draw(target, context);
            target.PopModelMatrix();
        }

        public void Initialize()
        {
            Controller.Bind(this);
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
