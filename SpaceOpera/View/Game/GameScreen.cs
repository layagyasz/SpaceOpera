using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Game.Overlay;
using SpaceOpera.View.Game.Panes;
using SpaceOpera.View.Game.Scenes;

namespace SpaceOpera.View.Game
{
    public class GameScreen : IRenderable, IDynamic
    {
        private static readonly long s_RefreshTime = 1000;

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public IController Controller { get; }
        public OverlaySet OverlaySet { get; }
        public DynamicUiGroup OverlayLayer { get; }
        public PaneSet PaneSet { get; }
        public DynamicUiGroup PaneLayer { get; }

        private long _time;

        public IGameScene? Scene { get; private set; }
        private Vector3 _bounds;

        public GameScreen(
            IController controller, OverlaySet overlaySet, DynamicUiGroup overlayLayer, PaneSet paneSet, DynamicUiGroup paneLayer)
        {
            Controller = controller;
            OverlaySet = overlaySet;
            OverlayLayer = overlayLayer;
            PaneSet = paneSet;
            PaneLayer = paneLayer;
        }

        public void ClearPanes()
        {
            PaneLayer.Clear();
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene?.Draw(target, context);
            OverlayLayer.Draw(target, context);
            PaneLayer.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            OverlayLayer.Initialize();
            PaneLayer.Initialize();
        }

        public void CloseOverlay(IOverlay overlay)
        {
            OverlayLayer.Remove(overlay);
        }

        public void OpenOverlay(IOverlay overlay)
        {
            if (!OverlayLayer.Contains(overlay))
            {
                OverlayLayer.Add(overlay);
            }
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
            OverlayLayer.Refresh();
            PaneLayer.Refresh();
            Scene?.Refresh();
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void ResizeContext(Vector3 bounds)
        {
            _bounds = bounds;
            Scene?.ResizeContext(bounds);
            foreach (var overlay in OverlaySet.GetOverlays())
            {
                overlay.ResizeContext(bounds);
            }
        }

        public void SetScene(IGameScene scene)
        {
            Scene = scene;
            Scene.ResizeContext(_bounds);
            Scene.Refresh();
            OverlayLayer.Parent = Scene;
            PaneLayer.Parent = Scene;
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
            OverlayLayer.Update(delta);
            PaneLayer.Update(delta);
        }
    }
}
