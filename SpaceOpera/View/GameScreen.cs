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

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IController Controller { get; }
        public UiGroup PaneLayer { get; }

        public EmpireOverlay EmpireOverlay { get; }

        private readonly PaneSet _paneSet;

        private long _time;

        public IGameScene? Scene { get; private set; }
        private Vector3 _bounds;

        public GameScreen(IController controller, EmpireOverlay empireOverlay, PaneSet paneSet, UiGroup paneLayer)
        {
            Controller = controller;
            EmpireOverlay = empireOverlay;
            _paneSet = paneSet;
            PaneLayer = paneLayer;
        }

        public void ClearPanes()
        {
            PaneLayer.Clear();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene?.Draw(target, context);
            EmpireOverlay.Draw(target, context);
            PaneLayer.Draw(target, context);
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
            PaneLayer.Initialize();
        }

        public void OpenPane(IGamePane pane, bool closeOpenPanes)
        {
            if (!PaneLayer.Contains(pane))
            {
                if (closeOpenPanes)
                {
                    ClearPanes();
                }
                pane.Position = 0.5f * (_bounds - pane.Size);
                PaneLayer.Add(pane);
            }
        }

        public void Refresh()
        {
            EmpireOverlay.Refresh();
            foreach (var pane in PaneLayer)
            {
                if (pane is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
            Scene?.Refresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
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
            Scene.Refresh();
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
            PaneLayer.Update(delta);
        }
    }
}
