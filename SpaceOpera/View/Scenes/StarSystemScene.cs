using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;
using SpaceOpera.View.StarSystemViews;

namespace SpaceOpera.View.Scenes
{
    public class StarSystemScene : GraphicsResource, IGameScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; set; }

        private StarSystemModel? _model;
        private StarSubSystemRig[]? _subSystems;
        private readonly SubRegionInteractor[] _interactors;
        private HighlightLayer<INavigable>? _highlightLayer;
        private readonly Skybox _skybox;

        public StarSystemScene(
            IElementController controller,
            ICamera camera,
            StarSystemModel model,
            StarSubSystemRig[] subSystems,
            SubRegionInteractor[] interactors,
            HighlightLayer<INavigable> highlightLayer,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            _model = model;
            _subSystems = subSystems;
            foreach (var subSystem in subSystems)
            {
                subSystem.Parent = this;
            }
            _interactors = interactors;
            _highlightLayer = highlightLayer;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            _model!.Dispose();
            _model = null;
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Dispose();
            }
            _subSystems = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            foreach (var interactor in _interactors)
            {
                context.Register(interactor);
            }

            _skybox.Draw(target, context);
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Draw(target, context);
            }
            _highlightLayer!.Draw(target, context);
            _model!.Draw(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _model!.Initialize();
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Initialize();
            }
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            _highlightLayer!.SetLayer(layer, highlight);
            foreach (var subSystem in _subSystems!)
            {
                subSystem.SetHighlight(layer, highlight);
            }
        }

        public void Update(long delta)
        {
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Update(delta);
            }
        }
    }
}
