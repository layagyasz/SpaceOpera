using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.View.Overlay;
using SpaceOpera.View.Panes;
using SpaceOpera.View.Scenes;

namespace SpaceOpera.View
{
    public class GameScreen : IRenderable
    {
        public IController Controller { get; }
        public EmpireOverlay EmpireOverlay { get; }

        private PaneSet _paneSet;
        private UiGroup _paneLayer;

        public IGameScene? Scene { get; private set; }
        private Vector3 _bounds;

        public GameScreen(IController controller, EmpireOverlay empireOverlay, PaneSet paneSet, UiGroup paneLayer)
        {
            Controller = controller;
            EmpireOverlay = empireOverlay;
            _paneSet = paneSet;
            _paneLayer = paneLayer;
        }

        public void ClearPanes()
        {
            _paneLayer.Clear();
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            Scene?.Draw(target, context);
            target.Flatten();
            context.Flatten();
            EmpireOverlay.Draw(target, context);
            _paneLayer.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            EmpireOverlay.Initialize();
            _paneLayer.Initialize();
        }

        public void OpenPane(GamePaneId id)
        {
            var pane = _paneSet.Get(id);
            pane.Position = 0.5f * (_bounds - pane.Size);
            ((PaneLayerController)_paneLayer.Controller).Add(pane);
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
            _paneLayer.Update(delta);
        }
    }
}
