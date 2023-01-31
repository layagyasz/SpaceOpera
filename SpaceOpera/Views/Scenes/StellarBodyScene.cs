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
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private readonly StellarBodyModel _stellarBodyModel;

        public StellarBodyScene(IElementController controller, ICamera camera, StellarBodyModel stellarBodyModel)
        {
            Controller = controller;
            Camera = camera;
            _stellarBodyModel = stellarBodyModel;
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
            _stellarBodyModel.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public void Update(long delta)
        {
            _stellarBodyModel.Update(delta);
        }
    }
}
