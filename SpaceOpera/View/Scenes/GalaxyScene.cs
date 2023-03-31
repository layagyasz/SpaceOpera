using Cardamom.Graphics.Camera;
using Cardamom.Graphics;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.FormationViews;
using SpaceOpera.View.GalaxyViews;

namespace SpaceOpera.View.Scenes
{
    public class GalaxyScene : GraphicsResource, IGameScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; }

        private InteractiveModel? _galaxyModel;
        private HighlightLayer<StarSystem, StarSystem>? _highlightLayer;
        private FormationLayer<StarSystem>? _formationLayer;
        private readonly Skybox _skybox;

        public GalaxyScene(
            IElementController controller,
            ICamera camera, 
            InteractiveModel galaxyModel,
            HighlightLayer<StarSystem, StarSystem> highlightLayer,
            FormationLayer<StarSystem> formationLayer,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            Camera.Changed += HandleCameraUpdate;
            _galaxyModel = galaxyModel;
            _galaxyModel.Parent = this;
            _highlightLayer = highlightLayer;
            _formationLayer = formationLayer;
            _formationLayer.Parent = this;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            Camera.Changed -= HandleCameraUpdate;
            _galaxyModel!.Dispose();
            _galaxyModel = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
            _formationLayer!.Dispose();
            _formationLayer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);

            _skybox.Draw(target, context);
            _highlightLayer!.Draw(target, context);
            _galaxyModel!.Draw(target, context);
            _formationLayer!.UpdateFromCamera(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();

            target.Flatten();
            context.Flatten();
            _formationLayer?.Draw(target, context);
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _galaxyModel!.Initialize();
            _highlightLayer!.Initialize();
            _formationLayer!.Initialize();
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight)
        {
            _highlightLayer!.SetLayer(layer, highlight);
        }

        public void Update(long delta)
        {
            _galaxyModel!.Update(delta);
            _highlightLayer!.Update(delta);
            _formationLayer!.Update(delta);
        }

        private void HandleCameraUpdate(object? sender, EventArgs e)
        {
            ((GalaxyModel)_galaxyModel!.GetModel()).Dirty();
            _formationLayer!.Dirty();
        }
    }
}
