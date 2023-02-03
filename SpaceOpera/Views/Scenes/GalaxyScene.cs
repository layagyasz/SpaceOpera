using Cardamom.Graphics.Camera;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Views.GalaxyViews;

namespace SpaceOpera.Views.Scenes
{
    public class GalaxyScene : IScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private readonly GalaxyModel _galaxyModel;

        public GalaxyScene(IElementController controller, ICamera camera, GalaxyModel galaxyModel)
        {
            Controller = controller;
            Camera = camera;
            _galaxyModel = galaxyModel;
        }

        public void Initialize()
        {
            _galaxyModel.Initialize();
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

            _galaxyModel.Draw(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public void Update(long delta)
        {
            _galaxyModel.Update(delta);
        }
    }
}
