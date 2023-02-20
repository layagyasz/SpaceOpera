using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Scenes.Highlights;

namespace SpaceOpera.View.StarSystemViews
{
    public class StarSubSystemRig : GraphicsResource, IRenderable
    {
        private static readonly float s_GameYearInMillis = 360000f;
        private static readonly int s_OrbitAccuracy = 20;
        private static readonly float s_OrbitPrecision = 0.01f;

        private readonly StellarBody _stellarBody;
        private readonly StarCalendar _calendar;
        private StarSubSystemView? _view;
        private readonly float _scale;

        private readonly float _offset;
        private readonly float _step;

        public StarSubSystemRig(StellarBody stellarBody, StarCalendar calendar, StarSubSystemView view, float scale)
        {
            _stellarBody = stellarBody;
            _calendar = calendar;
            _view = view;
            _scale = scale;
            _offset = MathHelper.TwoPi * stellarBody.Orbit.TimeOffset;
            _step = MathHelper.TwoPi * 0.001f * s_GameYearInMillis / stellarBody.GetYearLengthInMillis();
        }

        protected override void DisposeImpl()
        {
            _view!.Dispose();
            _view = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushModelMatrix(
                Matrix4.CreateTranslation(
                    _scale * _stellarBody.GetSolarOrbitPosition(
                        _stellarBody.GetSolarOrbitProgression(
                            _offset + _step * _calendar.GetMillis(), s_OrbitPrecision, s_OrbitAccuracy))));
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
