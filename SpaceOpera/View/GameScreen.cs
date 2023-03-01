using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.View.Overlay;
using SpaceOpera.View.Scenes;

namespace SpaceOpera.View
{
    public class GameScreen : IRenderable
    {
        public IController Controller { get; }
        public EmpireOverlay EmpireOverlay { get; }
        public IUiLayer PaneLayer { get; }

        public IGameScene? Scene { get; private set; }
        private Vector3 _bounds;

        public GameScreen(IController controller, EmpireOverlay empireOverlay, IUiLayer paneLayer)
        {
            Controller = controller;
            EmpireOverlay = empireOverlay;
            PaneLayer = paneLayer;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            Scene?.Draw(target, context);
            target.Flatten();
            context.Flatten();
            EmpireOverlay.Draw(target, context);
            PaneLayer.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            EmpireOverlay.Initialize();
            PaneLayer.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            Scene?.ResizeContext(bounds);
        }

        public void SetScene(IGameScene scene)
        {
            Scene = scene;
            Scene.ResizeContext(_bounds);
        }

        public void Update(long delta)
        {
            Scene?.Update(delta);
            EmpireOverlay.Update(delta);
            PaneLayer.Update(delta);
        }
    }
}
