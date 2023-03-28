using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.Panes;
using SpaceOpera.View.Scenes;

namespace SpaceOpera.Controller
{
    public class GameController : IController
    {
        private static readonly ISet<Type> s_SceneTypes = 
            new HashSet<Type>() { typeof(Galaxy), typeof(StarSystem), typeof(StellarBody) };

        private readonly UiWindow _window;
        private readonly World? _world;
        private readonly GameDriver _driver;
        private readonly Faction _faction;
        private readonly ViewFactory _viewFactory;
        private readonly ILogger _logger;
        private GameScreen? _screen;

        private Stack<IGameScene> _scenes = new();
        private EnumMap<HighlightLayerName, ICompositeHighlight> _currentHighlights = new();

        public GameController(
            UiWindow window, World? world, GameDriver driver, Faction faction, ViewFactory viewFactory, ILogger logger)
        {
            _window = window;
            _world = world;
            _driver = driver;
            _faction = faction;
            _viewFactory = viewFactory;
            _logger = logger;
        }

        public void Bind(object @object)
        {
            _screen = @object as GameScreen;
            if (_screen!.EmpireOverlay.ComponentController is IActionController controller)
            {
                controller.Interacted += HandleInteraction;
            }
            foreach (var pane in _screen.GetPanes())
            {
                if (pane.Controller is IActionController paneController)
                {
                    paneController.Interacted += HandleInteraction;
                }
            }
        }

        public void Unbind()
        {
            _screen = null;
            if (_screen!.EmpireOverlay.ComponentController is IActionController controller)
            {
                controller.Interacted -= HandleInteraction;
            }
            foreach (var pane in _screen.GetPanes())
            {
                if (pane.Controller is IActionController paneController)
                {
                    paneController.Interacted -= HandleInteraction;
                }
            }
        }

        public void PushScene(object sceneObject)
        {
            if (_screen?.Scene != null)
            {
                _scenes.Push(_screen.Scene);
            }
            ChangeSceneTo(sceneObject, /* cleanUp= */ false);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight)
        {
            if (highlight == null)
            {
                _currentHighlights.Remove(layer);
            }
            else
            {
                _currentHighlights[layer] = highlight;
            }
            _screen?.Scene?.SetHighlight(layer, highlight);
        }

        public void TryPopScene()
        {
            if(_scenes.TryPop(out var scene))
            {
                ChangeSceneTo(scene, /* cleanUp= */ true);
            }
        }

        private void ChangeSceneTo(object sceneObject, bool cleanUp)
        {
            if (sceneObject is Galaxy galaxy)
            {
                ChangeSceneTo(_viewFactory.SceneFactory.Create(galaxy, _world), cleanUp);
            }
            else if (sceneObject is StarSystem starSystem)
            {
                ChangeSceneTo(
                    _viewFactory.SceneFactory.Create(starSystem, _world?.Calendar ?? new(0)), cleanUp);
            }
            else if (sceneObject is StellarBody stellarBody)
            {
                ChangeSceneTo(_viewFactory.SceneFactory.Create(stellarBody), cleanUp);
            }
            else
            {
                throw new ArgumentException($"Unsupported sceneObject type: [{sceneObject.GetType()}]");
            }
        }

        private void ChangeSceneTo(IGameScene scene, bool cleanUp)
        {
            scene.Initialize();
            if (cleanUp)
            {
                _screen?.Scene?.Dispose();
            }
            if (_screen?.Scene?.Controller is IActionController oldSceneController)
            {
                oldSceneController.Interacted -= HandleInteraction;
            }
            if (scene.Controller is IActionController sceneController)
            {
                sceneController.Interacted += HandleInteraction;
            }
            foreach (var layer in Enum.GetValues(typeof(HighlightLayerName)).Cast<HighlightLayerName>())
            {
                if (_currentHighlights.TryGetValue(layer, out var highlight))
                {
                    scene.SetHighlight(layer, highlight);
                }
                else
                {
                    scene.SetHighlight(layer, null);
                }
            }
            _screen!.SetScene(scene);
            _window.SetFocus(scene);
        }

        public void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log(e.ToString());
            if (e.Key == Keys.Backspace)
            {
                TryPopScene();
                return;
            }
            if (e.Key == Keys.D0)
            {
                SetHighlight(HighlightLayerName.Background, null);
                SetHighlight(HighlightLayerName.Midground, null);
            }
            if (e.Key == Keys.D1)
            {
                SetHighlight(HighlightLayerName.Background, SimpleHighlight.Wrap(SubRegionHighlight.Create()));
                if (_world != null)
                {
                    SetHighlight(
                        HighlightLayerName.Midground, FactionHighlight.Create(_world, _viewFactory.BannerViewFactory));
                }
            }
            var @object = e.GetOnlyObject();
            if (e.Action != null)
            {
                if (@object is Design)
                {
                    OpenPane(GamePaneId.Designer, _world!, _faction, @object);
                    return;
                }
                var gameSpeed = GetGameSpeed(e.Action.Value);
                if (gameSpeed != null)
                {
                    _driver.SetGameSpeed(gameSpeed.Value);
                }
                var paneId = GetPane(e.Action.Value);
                if (paneId != GamePaneId.None)
                {
                    OpenPane(paneId, _world!, _faction);
                }
                return;
            }
            if (@object != null)
            {
                if (s_SceneTypes.Contains(@object.GetType()))
                {
                    PushScene(@object);
                    return;
                }
                if (@object is TransitRegion transit)
                {
                    ChangeSceneTo(transit.TransitSystem, /* cleanUp= */ true);
                }
            }
        }

        private void OpenPane(GamePaneId paneId, params object[] args)
        {
            _screen!.ClearPanes();
            var pane = _screen!.GetPane(paneId);
            pane.Populate(args);
            _screen!.OpenPane(pane);
        }
        
        private static GamePaneId GetPane(ActionId id)
        {
            return id switch
            {
                ActionId.Equipment => GamePaneId.Equipment,
                ActionId.Military => GamePaneId.Military,
                ActionId.MilitaryOrganization => GamePaneId.MilitaryOrganization,
                ActionId.Research => GamePaneId.Research,
                _ => GamePaneId.None,
            };
        }

        private static int? GetGameSpeed(ActionId id)
        {
            return id switch
            {
                ActionId.GameSpeedPause => 0,
                ActionId.GameSpeedNormal => 1,
                ActionId.GameSpeedFast => 8,
                _ => null,
            };
        }
    }
}
