using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Controller.Subcontrollers;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using SpaceOpera.View.Highlights;
using SpaceOpera.View.Panes;
using SpaceOpera.View.Panes.StellarBodyRegionPanes;
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

        private readonly Stack<IGameScene> _scenes = new();
        private readonly EnumMap<HighlightLayerName, ICompositeHighlight> _currentHighlights = new();
        private readonly HashSet<IFormationDriver> _selectedFormations = new();

        private ISubcontroller? _subcontroller;

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
            _screen.PaneLayer.ElementRemoved += HandlePaneClosed;
            foreach (var pane in _screen.GetPanes())
            {
                if (pane.Controller is IGamePaneController paneController)
                {
                    paneController.Interacted += HandleInteraction;
                    paneController.OrderCreated += HandleOrder;
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
                if (pane.Controller is IGamePaneController paneController)
                {
                    paneController.Interacted -= HandleInteraction;
                    paneController.OrderCreated -= HandleOrder;
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
                ChangeSceneTo(_viewFactory.SceneFactory.Create(_world, galaxy), cleanUp);
            }
            else if (sceneObject is StarSystem starSystem)
            {
                ChangeSceneTo(
                    _viewFactory.SceneFactory.Create(_world, starSystem, _world?.Calendar ?? new(0)), cleanUp);
            }
            else if (sceneObject is StellarBody stellarBody)
            {
                ChangeSceneTo(_viewFactory.SceneFactory.Create(_world, stellarBody), cleanUp);
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

        private void ChangeSubcontrollerTo(ISubcontroller? subcontroller)
        {
            if (_subcontroller != null)
            {
                _subcontroller.OrderCreated -= HandleOrder;
            }
            _subcontroller = subcontroller;
            if (_subcontroller != null)
            {
                _subcontroller.OrderCreated += HandleOrder;
            }
        }
        
        private void HandleOrder(object? sender, IOrder e)
        {
            // Orders that require manual confirmation.
            if (e is BuildOrder)
            {
                OpenPane(GamePaneId.OrderConfirmation, /* closeOpenPanes= */ false, e);
            }
            else
            {
                ExecuteOrder(e);
            }
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log(e.ToString());

            if (HandleKeyInteraction(e.Key))
            {
                return;
            }
            if (_subcontroller?.HandleInteraction(e) ?? false)
            {
                return;
            }

            var representative = e.Objects.FirstOrDefault()?.GetType() ?? null;
            if (representative == null)
            {
                HandleNullInteraction(e);
            }
            else
            {
                HandleObjectInteraction(representative, e);
            }
        }

        private bool HandleKeyInteraction(Keys? key)
        {
            if (key == Keys.Backspace)
            {
                TryPopScene();
                return true;
            }
            if (key == Keys.D0)
            {
                SetHighlight(HighlightLayerName.Background, null);
                SetHighlight(HighlightLayerName.Midground, null);
                return true;
            }
            if (key == Keys.D1)
            {
                SetHighlight(HighlightLayerName.Background, SimpleHighlight.Wrap(SubRegionHighlight.Create()));
                if (_world != null)
                {
                    SetHighlight(
                        HighlightLayerName.Midground, FactionHighlight.Create(_world, _viewFactory.BannerViewFactory));
                }
                return true;
            }
            return false;
        }

        private void HandleNullInteraction(UiInteractionEventArgs e)
        {
            if (e.Action == null)
            {
                return;
            }
            var gameSpeed = GetGameSpeed(e.Action.Value);
            if (gameSpeed != null)
            {
                _driver.SetGameSpeed(gameSpeed.Value);
                return;
            }
            var paneId = GetPane(e.Action.Value);
            if (paneId != GamePaneId.None)
            {
                OpenPane(paneId, /* closeOpenPanes= */ true, _world!, _faction);
                return;
            }
        }

        private void HandleObjectInteraction(Type type, UiInteractionEventArgs e)
        {
            if (type.IsAssignableTo(typeof(IFormationDriver)))
            {
                if (e.Action == ActionId.Select)
                {
                    SelectFormations(e.Objects.Cast<IFormationDriver>());
                    return;
                }
                if (e.Action == ActionId.Battle)
                {
                    OpenPane(
                        GamePaneId.Battle, 
                        /* closeOpenPanes= */ true,
                        e.Objects.Cast<IFormationDriver>()
                            .Select(x => x.Formation)
                            .Select(_world!.BattleManager.GetBattle)
                            .Where(x => x != null)
                            .FirstOrDefault());
                }
            }
            if (type.IsAssignableTo(typeof(IFormationDriver)) && e.Action == ActionId.Unselect)
            {
                UnselectFormations(e.Objects.Cast<IFormationDriver>());
                return;
            }
            if (type.IsAssignableTo(typeof(IOrder)) && e.Action == ActionId.Confirm)
            {
                ExecuteOrder((IOrder)e.GetOnlyObject()!);
                return;
            }
            if (type.IsAssignableTo(typeof(Design)) && e.Action == ActionId.Edit)
            {
                OpenPane(GamePaneId.Designer, /* closeOpenPanes= */ true, _world!, _faction, e.GetOnlyObject()!);
                return;
            }
            if (type.IsAssignableTo(typeof(StellarBodySubRegion)) && e.Button == MouseButton.Left)
            {
                var subRegion = (StellarBodySubRegion)e.GetOnlyObject()!;
                OpenPane(
                    GamePaneId.StellarBodyRegion,
                    /* closeOpenPanes= */ true,
                    _world!,
                    _faction,
                    subRegion.ParentRegion!);
                SetHighlight(
                    HighlightLayerName.Foreground,
                    SimpleHighlight.Wrap(new StellarBodyRegionHighlight(subRegion.ParentRegion!)));
                return;
            }
            if (s_SceneTypes.Contains(type))
            {
                PushScene(e.GetOnlyObject()!);
                return;
            }
            if (type.IsAssignableTo(typeof(TransitRegion)))
            {
                var transit = (TransitRegion)e.GetOnlyObject()!;
                ChangeSceneTo(transit.TransitSystem, /* cleanUp= */ true);
            }
        }

        private void SelectFormations(IEnumerable<IFormationDriver> drivers)
        {
            OpenPane(GamePaneId.Formation, /* closeOpenPanes= */ true, drivers);
            _selectedFormations.Clear();
            foreach (var driver in drivers)
            {
                _selectedFormations.Add(driver);
            }
            if (_selectedFormations.Count > 0 && _selectedFormations.First().GetType() == typeof(FleetDriver))
            {
                ChangeSubcontrollerTo(new FleetSubcontroller(drivers.Cast<FleetDriver>()));
                SetHighlight(
                    HighlightLayerName.Foreground,
                    SimpleHighlight.Wrap(new FormationHighlight(_selectedFormations)));
            }
        }

        private void UnselectFormations(IEnumerable<IFormationDriver> drivers)
        {
            foreach (var driver in drivers)
            {
                _selectedFormations.Remove(driver);
            }
            if (_selectedFormations.Count == 0)
            {
                ChangeSubcontrollerTo(null);
                SetHighlight(HighlightLayerName.Foreground, null);
            }
        }

        private void ExecuteOrder(IOrder order)
        {
            var result = _driver.Execute(order);
            _logger.AtInfo().Log(order.ToString() + " " + result);
            _screen!.Refresh();
        }

        private void HandlePaneClosed(object? sender, ElementEventArgs e)
        {
            if (e.Element is StellarBodyRegionPane)
            {
                SetHighlight(HighlightLayerName.Foreground, null);
            }
        }

        private void OpenPane(GamePaneId paneId, bool closeOpenPanes, params object?[] args)
        {
            var pane = _screen!.GetPane(paneId);
            pane.Populate(args);
            _screen!.OpenPane(pane, closeOpenPanes);
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
