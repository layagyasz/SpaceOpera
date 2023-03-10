using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.View.Common;
using SpaceOpera.View.Common.Highlights;
using SpaceOpera.View.StarViews;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera.View.Scenes
{
    public class StellarBodyScene : GraphicsResource, IGameScene
    {
        private static readonly float s_StarSpeed = 0.00005f * MathF.PI;

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private StellarBodyModel? _stellarBodyModel;
        private readonly RenderShader _surfaceShader;
        private readonly RenderShader _atmosphereShader;
        private readonly Light _light;
        private StarBuffer? _star;
        private readonly Skybox _skybox;
        private long _rotation = 0;

        public StellarBodyScene(
            IElementController controller, 
            ICamera camera, 
            StellarBodyModel stellarBodyModel,
            RenderShader surfaceShader,
            RenderShader atmosphereShader,
            Light light,
            StarBuffer star,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            _stellarBodyModel = stellarBodyModel;
            _surfaceShader = surfaceShader;
            _atmosphereShader = atmosphereShader;
            _light = light;
            _star = star;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            _star!.Dispose();
            _star = null;
            _stellarBodyModel!.Dispose();
            _stellarBodyModel = null;
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

            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _stellarBodyModel!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight highlight) { }

        public void Update(long delta)
        {
            _rotation += delta;
            _stellarBodyModel!.Update(delta);
        }
    }
}
