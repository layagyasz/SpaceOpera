using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using Cardamom.Utils.Suppliers;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Game.Panes;
using SpaceOpera.Controller.Game.Subcontrollers;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Events;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Game;
using SpaceOpera.View.Game.Highlights;
using SpaceOpera.View.Game.Overlay;
using SpaceOpera.View.Game.Panes;
using SpaceOpera.View.Game.Panes.FormationPanes;
using SpaceOpera.View.Game.Panes.Forms;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;
using SpaceOpera.View.Game.Scenes;

namespace SpaceOpera.Controller.Game
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

        private IInterceptor? _interceptor;
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
            foreach (var overlay in _screen!.OverlaySet.GetOverlays())
            {
                if (overlay is UiCompoundComponent component)
                {
                    if (component.ComponentController is IActionController overlayController)
                    {
                        overlayController.Interacted += HandleInteraction;
                    }
                }
            }
            _screen!.PaneLayer.ElementRemoved += HandlePaneClosed;
            foreach (var pane in _screen.PaneSet.GetPanes())
            {
                if (pane.Controller is IGamePaneController paneController)
                {
                    paneController.Interacted += HandleInteraction;
                    paneController.InterceptorCreated += HandleInterceptorCreated;
                    paneController.InterceptorCancelled += HandleInterceptorCancelled;
                    paneController.OrderCreated += HandleOrder;
                    paneController.PopupCreated += HandlePopup;
                }
            }
        }

        public void Unbind()
        {
            _screen = null;
            foreach (var overlay in _screen!.OverlaySet.GetOverlays())
            {
                if (overlay is UiCompoundComponent component)
                {
                    if (component.ComponentController is IActionController overlayController)
                    {
                        overlayController.Interacted -= HandleInteraction;
                    }
                }
            }
            foreach (var pane in _screen!.PaneSet.GetPanes())
            {
                if (pane.Controller is IGamePaneController paneController)
                {
                    paneController.Interacted -= HandleInteraction;
                    paneController.InterceptorCreated -= HandleInterceptorCreated;
                    paneController.InterceptorCancelled -= HandleInterceptorCancelled;
                    paneController.OrderCreated -= HandleOrder;
                    paneController.PopupCreated -= HandlePopup;
                }
            }
            foreach (var scene in _scenes)
            {
                scene.Dispose();
            }
            _scenes.Clear();
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
            IGameScene scene;
            if (sceneObject is Galaxy galaxy)
            {
                scene = _viewFactory.SceneFactory.Create(_world, galaxy);
            }
            else if (sceneObject is StarSystem starSystem)
            {
                scene = _viewFactory.SceneFactory.Create(_world, starSystem, _world?.Calendar ?? new(0));
            }
            else if (sceneObject is StellarBody stellarBody)
            {
                scene = _viewFactory.SceneFactory.Create(_world, stellarBody);
            }
            else
            {
                throw new ArgumentException($"Unsupported sceneObject type: [{sceneObject.GetType()}]");
            }
            scene.Initialize();
            ChangeSceneTo(scene, cleanUp);
        }

        private void ChangeSceneTo(IGameScene scene, bool cleanUp)
        {
            OpenOverlay(OverlayId.Game, _world);
            OpenOverlay(OverlayId.Empire, _world, _faction);
            OpenOverlay(OverlayId.Event, _world, _faction);
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
            if (scene is StarSystemScene starSystemScene)
            {
                OpenOverlay(OverlayId.StarSystem, starSystemScene.StarSystem);
            }
            else
            {
                CloseOverlay(OverlayId.StarSystem);
            }
            _screen!.SetScene(scene);
            _window.SetFocus(scene);
        }

        private void ChangeSubcontrollerTo(ISubcontroller? subcontroller)
        {
            _logger.AtInfo().Log($"Set subcontroller {subcontroller}");
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

        private void HandleInterceptorCreated(object? sender, IInterceptor e)
        {
            _logger.AtInfo().Log($"Create {e}");
            _interceptor = e;
        }

        private void HandleInterceptorCancelled(object? sender, IInterceptor e)
        {
            _logger.AtInfo().Log($"Cancel {e}");
            if (_interceptor == e)
            {
                _interceptor = null;
            }
        }
        
        private void HandleOrder(object? sender, IOrder e)
        {
            // Orders that require manual confirmation.
            if (e is BuildOrder)
            {
                var promise = new Promise<FormValue>();
                promise.Finished += HandleOrderDecision;
                promise.Canceled += HandleOrderDecisionCanceled;
                OpenPane(
                    GamePaneId.Form,
                    /* closeOpenPanes= */ false,
                    _world,
                    FormHelper.ForOrder(e),
                    promise);
                return;
            }
            else
            {
                ExecuteOrder(e);
            }
        }

        private void HandlePopup(object? sender, PopupEventArgs e)
        {
            OpenPane(GamePaneId.Form, /* closeOpenPanes= */ false, _world, e.Layout, e.Promise);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log($"{_subcontroller?.GetType()} {e}");

            if (HandleKeyInteraction(e.Key))
            {
                return;
            }
            if (_interceptor?.Intercept(e) ?? false)
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
            if (key == Keys.D2)
            {
                SetHighlight(HighlightLayerName.Background, SimpleHighlight.Wrap(SubRegionHighlight.Create()));
                if (_world != null)
                {
                    SetHighlight(HighlightLayerName.Midground, FrontHighlight.Create(_faction, _world));
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
            var gameSpeed = ActionIdMapper.ToGameSpeed(e.Action.Value);
            if (gameSpeed != null)
            {
                _driver.SetGameSpeed(gameSpeed.Value);
                return;
            }
            var paneId = ActionIdMapper.ToPaneId(e.Action.Value);
            if (paneId != GamePaneId.Unknown)
            {
                OpenPane(paneId, /* closeOpenPanes= */ true, _world!, _faction);
                return;
            }
        }

        private void HandleObjectInteraction(Type type, UiInteractionEventArgs e)
        {
            if (type.IsAssignableTo(typeof(Type)) 
                && (Type)e.GetOnlyObject()! == typeof(PersistentRoute) 
                && e.Action == ActionId.Add)
            {
                OpenPane(GamePaneId.LogisticsRoute, /* closeOpenPanes= */ true, _world, _faction, null);
                return;
            }
            if (type.IsAssignableTo(typeof(AtomicFormationDriver)))
            {
                if (e.Action == ActionId.Battle)
                {
                    OpenPane(
                        GamePaneId.Battle, 
                        /* closeOpenPanes= */ true,
                        e.Objects.Cast<AtomicFormationDriver>()
                            .Select(x => x.AtomicFormation)
                            .Select(_world!.Battles.GetBattle)
                            .Where(x => x != null)
                            .FirstOrDefault());
                    return;
                }
            }
            if (type.IsAssignableTo(typeof(IEvent)))
            {
                var @event = (IEvent)e.GetOnlyObject()!;
                if (e.Action == ActionId.Select)
                {
                    var promise = new Promise<FormValue>();
                    promise.Finished += HandleEventDecision;
                    promise.Canceled += HandleEventDecisionCanceled;
                    OpenPane(
                        GamePaneId.Form,
                        /* closeOpenPanes= */ true,
                        _world,
                        FormHelper.ForEvent(@event), 
                        promise);
                    return;
                }
                if (e.Action == ActionId.Ignore && @event is NotificationBase notification)
                {
                    ExecuteOrder(new DecideEventOrder(@event, 0));
                }
            }
            if (type.IsAssignableTo(typeof(IFormationDriver)))
            {
                var assigment =
                    e.Action == null ? AssignmentType.Unknown : ActionIdMapper.ToAssignmentType(e.Action.Value);
                if (assigment != AssignmentType.Unknown)
                {
                    foreach (var driver in e.Objects.Cast<IFormationDriver>())
                    {
                        ExecuteOrder(new SetAssignmentOrder(driver, assigment));
                    }
                    return;
                }
                if (e.Action == ActionId.Select || e.Button == MouseButton.Left)
                {
                    SelectFormations(e.Objects.Cast<IFormationDriver>());
                    return;
                }
                if (e.Action == ActionId.Unselect)
                {
                    UnselectFormations(e.Objects.Cast<IFormationDriver>());
                    return;
                }
            }
            if (type.IsAssignableTo(typeof(IProject)) && e.Action == ActionId.Cancel)
            {
                ExecuteOrder(new CancelProjectOrder((IProject)e.GetOnlyObject()!));
                return;
            }
            if (type == typeof(ComponentType) && e.Action == ActionId.Add)
            {
                OpenPane(
                    GamePaneId.Designer,
                    /* closeOpenPanes= */ true,
                    _world!,
                    _faction, 
                    null, 
                    _world!.AutoDesigner.GetTemplate((ComponentType)e.GetOnlyObject()!));
                return;
            }
            if (type.IsAssignableTo(typeof(Design)) && e.Action == ActionId.Edit)
            {
                var design = (Design)e.GetOnlyObject()!;
                OpenPane(
                    GamePaneId.Designer,
                    /* closeOpenPanes= */ true, 
                    _world!, 
                    _faction,
                    e.GetOnlyObject()!,
                    design.Configuration.Template);
                return;
            }
            if (type.IsAssignableTo(typeof(DiplomaticRelation)) && e.Action == ActionId.Select)
            {
                var diplomaticRelation = (DiplomaticRelation)e.GetOnlyObject()!;
                OpenPane(GamePaneId.Diplomacy, /* closeOpenPanes= */ true, _world!, _faction, diplomaticRelation);
                return;
            }
            if (e.Button == MouseButton.Left || e.Action == ActionId.Select)
            {
                if (type.IsAssignableTo(typeof(StellarBodySubRegion)))
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
                if (type.IsAssignableTo(typeof(EconomicZoneHolding))) {
                    var holding = (EconomicZoneHolding)e.GetOnlyObject()!;
                    OpenPane(
                        GamePaneId.StellarBody, /* closeOpenPanes= */ true, _world!, _faction, holding.StellarBody);
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
        }

        private void SelectFormations(IEnumerable<IFormationDriver> drivers)
        {
            OpenPane(GamePaneId.Formation, /* closeOpenPanes= */ true, drivers);
            _selectedFormations.Clear();
            foreach (var driver in drivers)
            {
                _selectedFormations.Add(driver);
            }
            if (_selectedFormations.Count > 0)
            {
                if (_selectedFormations.First().GetType() == typeof(ArmyDriver))
                {
                    ChangeSubcontrollerTo(new ArmySubcontroller(_selectedFormations.Cast<ArmyDriver>()));
                }
                if (_selectedFormations.First().GetType() == typeof(FleetDriver))
                {
                    ChangeSubcontrollerTo(new FleetSubcontroller(_selectedFormations.Cast<FleetDriver>()));
                }
                if (_selectedFormations.First().GetType() == typeof(DivisionDriver))
                {
                    ChangeSubcontrollerTo(new DivisionSubcontroller(_selectedFormations.Cast<DivisionDriver>()));
                }
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
            else
            {
                if (_selectedFormations.First().GetType() == typeof(FleetDriver))
                {
                    ChangeSubcontrollerTo(new FleetSubcontroller(_selectedFormations.Cast<FleetDriver>()));
                }
                if (_selectedFormations.First().GetType() == typeof(DivisionDriver))
                {
                    ChangeSubcontrollerTo(new DivisionSubcontroller(_selectedFormations.Cast<DivisionDriver>()));
                }
            }
        }

        private void ExecuteOrder(IOrder order)
        {
            var result = _world?.Execute(order);
            _logger.AtInfo().Log(order.ToString() + " " + result);
            _screen!.Refresh();
        }

        private void HandleEventDecision(object? sender, EventArgs e)
        {
            Promise<FormValue> promise = (Promise<FormValue>)sender!;
            promise.Finished -= HandleEventDecision;
            promise.Canceled -= HandleEventDecisionCanceled;
            _world!.Execute(new DecideEventOrder((IEvent)promise.Get()["event"]!, (int)promise.Get()["decisionId"]!));
        }

        private void HandleEventDecisionCanceled(object? sender, EventArgs e)
        {
            Promise<FormValue> promise = (Promise<FormValue>)sender!;
            promise.Finished -= HandleEventDecision;
            promise.Canceled -= HandleEventDecisionCanceled;
        }

        private void HandleOrderDecision(object? sender, EventArgs e)
        {
            Promise<FormValue> promise = (Promise<FormValue>)sender!;
            promise.Finished -= HandleOrderDecision;
            promise.Canceled -= HandleOrderDecisionCanceled;
            if ((int)promise.Get()["decisionId"]! == 0)
            {
                ExecuteOrder((IOrder)promise.Get()["order"]!);
            }
        }

        private void HandleOrderDecisionCanceled(object? sender, EventArgs e)
        {
            Promise<FormValue> promise = (Promise<FormValue>)sender!;
            promise.Finished -= HandleOrderDecision;
            promise.Canceled -= HandleOrderDecisionCanceled;
        }

        private void HandlePaneClosed(object? sender, ElementEventArgs e)
        {
            if (e.Element is FormationPane)
            {
                UnselectFormations(_selectedFormations);
            }
            if (e.Element is StellarBodyRegionPane)
            {
                SetHighlight(HighlightLayerName.Foreground, null);
            }
        }

        private void CloseOverlay(OverlayId overlayId)
        {
            _screen!.CloseOverlay(_screen!.OverlaySet.Get(overlayId));
        }

        private void OpenOverlay(OverlayId overlayId, params object?[] args)
        {
            var overlay = _screen!.OverlaySet.Get(overlayId);
            overlay.Populate(args);
            _screen!.OpenOverlay(overlay);
        }

        private void OpenPane(GamePaneId paneId, bool closeOpenPanes, params object?[] args)
        {
            var pane = _screen!.PaneSet.Get(paneId);
            pane.Populate(args);
            _screen!.OpenPane(pane, closeOpenPanes);
        }
    }
}
