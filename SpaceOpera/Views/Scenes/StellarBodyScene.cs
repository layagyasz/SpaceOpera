using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class StellarBodyScene : IScene
    {
        private static Vector4 s_LightPosition = new(0, 0, -1000, 1);
        private static float s_LightSpeed = 0.0001f * MathF.PI;

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private readonly StellarBodyModel _stellarBodyModel;
        private readonly RenderShader _surfaceShader;
        private long _rotation = 0;

        public StellarBodyScene(
            IElementController controller, 
            ICamera camera, 
            StellarBodyModel stellarBodyModel,
            RenderShader surfaceShader)
        {
            Controller = controller;
            Camera = camera;
            _stellarBodyModel = stellarBodyModel;
            _surfaceShader = surfaceShader;
        }

        public void Initialize()
        {
            _stellarBodyModel.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);

            _surfaceShader.SetVector3(
                "light_position", (s_LightPosition * Matrix4.CreateRotationY(_rotation * s_LightSpeed)).Xyz);
            _surfaceShader.SetVector3("eye_position", Camera.Position);
            _surfaceShader.SetFloat("light_intensity", 0.5f);
            _surfaceShader.SetFloat("ambient", 0.1f);
            _stellarBodyModel.Draw(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public void Update(long delta)
        {
            _rotation += delta;
            _stellarBodyModel.Update(delta);
        }
    }
}
