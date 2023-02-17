using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.View.Scenes;

namespace SpaceOpera.View
{
    public class GameScreen : Screen
    {
        public IGameScene? Scene { get; private set; }
        private Vector3 _bounds;

        public GameScreen(IController controller, IEnumerable<IUiLayer> uiLayers)
            : base(controller, uiLayers) { }

        public override void Draw(RenderTarget target, UiContext context)
        {
            Scene?.Draw(target, context);
            target.Flatten();
            context.Flatten();
            foreach (var layer in _uiLayers)
            {
                layer.Draw(target, context);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            Scene?.ResizeContext(bounds);
        }

        public void SetScene(IGameScene scene)
        {
            Scene?.Dispose();
            Scene = scene;
            Scene.ResizeContext(_bounds);
        }

        public override void Update(long delta)
        {
            Scene?.Update(delta);
            base.Update(delta);
        }
    }
}
