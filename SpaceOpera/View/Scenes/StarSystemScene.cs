using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.FormationViews;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.StarSystemViews;

namespace SpaceOpera.View.Scenes
{
    public class StarSystemScene : GraphicsResource, IGameScene
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public float? OverrideDepth { get; set; }
        public ICamera Camera { get; set; }

        private StarSystemModel? _model;
        private readonly RenderShader _surfaceShader;
        private readonly RenderShader _atmosphereShader;
        private readonly Light _light;
        private StarSubSystemRig[]? _subSystems;
        private readonly SubRegionInteractor[] _interactors;
        private HighlightLayer<INavigable, INavigable>? _highlightLayer;
        private FormationLayer<object>? _formationLayer;
        private FormationSubLayer<object>? _formationSubLayer;
        private readonly Skybox _skybox;

        public StarSystemScene(
            IElementController controller,
            ICamera camera,
            StarSystemModel model,
            RenderShader surfaceShader,
            RenderShader atmosphereShader,
            Light light,
            StarSubSystemRig[] subSystems,
            SubRegionInteractor[] interactors,
            HighlightLayer<INavigable, INavigable> highlightLayer,
            FormationLayer<object> formationLayer,
            FormationSubLayer<object> formationSubLayer,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            Camera.Changed += HandleCameraUpdate;
            _model = model;
            _surfaceShader = surfaceShader;
            _atmosphereShader = atmosphereShader;
            _light = light;
            _subSystems = subSystems;
            foreach (var subSystem in subSystems)
            {
                subSystem.Parent = this;
            }
            _interactors = interactors;
            foreach (var interactor in interactors)
            {
                interactor.Parent = this;
            }
            _highlightLayer = highlightLayer;
            _formationLayer = formationLayer;
            _formationLayer.Parent = this;
            _formationSubLayer = formationSubLayer;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            Camera.Changed -= HandleCameraUpdate;
            _model!.Dispose();
            _model = null;
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Dispose();
            }
            _subSystems = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
            _formationLayer!.Dispose();
            _formationLayer = null;
            _formationSubLayer = null;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            foreach (var interactor in _interactors)
            {
                context.Register(interactor);
            }

            _surfaceShader.SetVector3("light_position", _light.Position);
            _surfaceShader.SetVector3("eye_position", Camera.Position);
            _surfaceShader.SetColor("light_color",_light.Color);
            _surfaceShader.SetFloat("light_luminance", _light.Luminance);
            _surfaceShader.SetFloat("light_attenuation", _light.Attenuation);
            _surfaceShader.SetFloat("ambient", 0.5f);
            _atmosphereShader.SetVector3("light_position", _light.Position);
            _atmosphereShader.SetVector3("eye_position", Camera.Position);
            _atmosphereShader.SetColor("light_color", _light.Color);
            _atmosphereShader.SetFloat("light_luminance", _light.Luminance);
            _atmosphereShader.SetFloat("light_attenuation", _light.Attenuation);

            _skybox.Draw(target, context);
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Draw(target, context);
            }
            _highlightLayer!.Draw(target, context);
            _model!.Draw(target, context);
            _formationSubLayer!.UpdateFromCamera(target);

            target.PopProjectionMatrix();
            target.PopViewMatrix();

            target.Flatten();
            context.Flatten();

            _formationLayer!.Draw(target, context);
            context.Flatten();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _model!.Initialize();
            _formationLayer!.Initialize();
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Initialize();
            }
            Controller.Bind(this);
        }

        public void Refresh()
        {
            _formationLayer!.Refresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight)
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
            _highlightLayer!.Update(delta);
            _formationLayer!.Update(delta);
        }

        private void HandleCameraUpdate(object? sender, EventArgs e)
        {
            _model!.Dirty();
            _formationLayer!.Dirty();
        }
    }
}
