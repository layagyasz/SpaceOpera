using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.FormationViews;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.StarViews;

namespace SpaceOpera.View.Scenes
{
    public class StellarBodyScene : GraphicsResource, IGameScene
    {
        private static readonly float s_StarSpeed = 0.00005f * MathF.PI;

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private InteractiveModel? _stellarBodyModel;
        private readonly RenderShader _surfaceShader;
        private readonly RenderShader _atmosphereShader;
        private readonly Light _light;
        private StarBuffer? _star;
        private HighlightLayer<StellarBodySubRegion, StellarBodySubRegion>? _surfaceHighlightLayer;
        private HighlightLayer<StationaryOrbitRegion, StellarBodySubRegion>? _orbitHighlightLayer;
        private FormationLayer<INavigable>? _formationLayer;
        private readonly Skybox _skybox;
        private long _rotation = 0;

        public StellarBodyScene(
            IElementController controller, 
            ICamera camera, 
            InteractiveModel stellarBodyModel,
            RenderShader surfaceShader,
            RenderShader atmosphereShader,
            Light light,
            StarBuffer star,
            HighlightLayer<StellarBodySubRegion, StellarBodySubRegion> surfaceHighlightLayer,
            HighlightLayer<StationaryOrbitRegion, StellarBodySubRegion> orbitHighlightLayer,
            FormationLayer<INavigable> formationLayer,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            Camera.Changed += HandleCameraChange;
            _stellarBodyModel = stellarBodyModel;
            _stellarBodyModel.Parent = this;
            _surfaceShader = surfaceShader;
            _atmosphereShader = atmosphereShader;
            _light = light;
            _star = star;
            _surfaceHighlightLayer = surfaceHighlightLayer;
            _orbitHighlightLayer = orbitHighlightLayer;
            _formationLayer = formationLayer;
            _formationLayer.Parent = null;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            Camera.Changed -= HandleCameraChange;
            _star!.Dispose();
            _star = null;
            _stellarBodyModel!.Dispose();
            _stellarBodyModel = null;
            _surfaceHighlightLayer!.Dispose();
            _surfaceHighlightLayer = null;
            _orbitHighlightLayer!.Dispose();
            _orbitHighlightLayer = null;
            _formationLayer!.Dispose();
            _formationLayer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);

            var sceneMatrix = Matrix4.CreateRotationY(_rotation * s_StarSpeed);
            target.PushModelMatrix(sceneMatrix);
            _skybox.Draw(target, context);
            _star!.Dirty();
            _star!.Draw(target, context);
            target.PopModelMatrix();

            var starPosition = _star!.Get(0).Position;
            var lightPosition = (new Vector4(starPosition, 1) * sceneMatrix).Xyz;
            _surfaceShader.SetVector3("light_position", lightPosition);
            _surfaceShader.SetVector3("eye_position", Camera.Position);
            _surfaceShader.SetColor("light_color", _light.Color);
            _surfaceShader.SetFloat("light_luminance", _light.Luminance);
            _surfaceShader.SetFloat("light_attenuation", _light.Attenuation);
            _surfaceShader.SetFloat("ambient", 0.5f);
            _atmosphereShader.SetVector3("light_position", lightPosition);
            _atmosphereShader.SetVector3("eye_position", Camera.Position);
            _atmosphereShader.SetColor("light_color", _star!.Get(0).Color);
            _atmosphereShader.SetFloat("light_luminance", _light.Luminance);
            _atmosphereShader.SetFloat("light_attenuation", _light.Attenuation);

            _stellarBodyModel!.Draw(target, context);
            _surfaceHighlightLayer!.Draw(target, context);
            _orbitHighlightLayer!.Draw(target, context);
            _formationLayer!.UpdateFromCamera(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();

            _formationLayer.Draw(target, context);

            target.Flatten();
            context.Flatten();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _stellarBodyModel!.Initialize();
            _formationLayer!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight)
        {
            _surfaceHighlightLayer!.SetLayer(layer, highlight);
            _orbitHighlightLayer!.SetLayer(layer, highlight);
        }

        public void Update(long delta)
        {
            _rotation += delta;
            _stellarBodyModel!.Update(delta);
        }

        private void HandleCameraChange(object? sender, EventArgs e)
        {
            _formationLayer!.Dirty();
        }
    }
}
