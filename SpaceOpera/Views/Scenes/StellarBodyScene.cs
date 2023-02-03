using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Views.StarViews;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views.Scenes
{
    public class StellarBodyScene : GraphicsResource, IScene
    {
        private static readonly float s_StarSpeed = 0.00005f * MathF.PI;

        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private readonly StellarBodyModel _stellarBodyModel;
        private readonly RenderShader _surfaceShader;
        private StarBuffer? _star;
        private long _rotation = 0;

        public StellarBodyScene(
            IElementController controller, 
            ICamera camera, 
            StellarBodyModel stellarBodyModel,
            RenderShader surfaceShader,
            StarBuffer star)
        {
            Controller = controller;
            Camera = camera;
            _stellarBodyModel = stellarBodyModel;
            _surfaceShader = surfaceShader;
            _star = star;
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

            var sceneMatrix = Matrix4.CreateRotationY(_rotation * s_StarSpeed);
            target.PushModelMatrix(sceneMatrix);
            _star!.Dirty();
            _star!.Draw(target, context);
            target.PopModelMatrix();

            var starPosition = _star!.Get(0).Position;
            _surfaceShader.SetVector3("light_position", (new Vector4(starPosition, 1) * sceneMatrix).Xyz);
            _surfaceShader.SetVector3("eye_position", Camera.Position);
            _surfaceShader.SetColor("light_color", _star!.Get(0).Color);
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

        protected override void DisposeImpl()
        {
            _star!.Dispose();
            _star = null;
        }
    }
}
