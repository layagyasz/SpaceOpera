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
    public class GameScreen : IRenderable, IDynamic
    {
        private static readonly long s_RefreshTime = 1000;

        public IController Controller { get; }
        public EmpireOverlay EmpireOverlay { get; }

        private PaneSet _paneSet;
        private UiGroup _paneLayer;

        private long _time;

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

        public IGamePane GetPane(GamePaneId id)
        {
            return _paneSet.Get(id);
        }

        public IEnumerable<IGamePane> GetPanes()
        {
            return _paneSet.GetPanes();
        }

        public void Initialize()
        {
            Controller.Bind(this);
            EmpireOverlay.Initialize();
            EmpireOverlay.CalendarOverlay.SetGameSpeed(ActionId.GameSpeedNormal);
            _paneLayer.Initialize();
        }

        public void OpenPane(IGamePane pane)
        {
            pane.Position = 0.5f * (_bounds - pane.Size);
            _paneLayer.Add(pane);
        }

        public void Refresh()
        {
            EmpireOverlay.Refresh();
            foreach (var pane in _paneLayer)
            {
                if (pane is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
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
            EmpireOverlay.Parent = Scene;
        }

        public void Update(long delta)
        {
            _time += delta;
            if (_time > s_RefreshTime)
            {
                _time = 0;
                Refresh();
            }


            Scene?.Update(delta);
            EmpireOverlay.Update(delta);
            _paneLayer.Update(delta);
        }
    }
}
