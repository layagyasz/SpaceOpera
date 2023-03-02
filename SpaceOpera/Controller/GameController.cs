using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Overlay;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using SpaceOpera.View.Common.Highlights;
using SpaceOpera.View.Overlay;
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
        private readonly ViewFactory _viewFactory;
        private readonly ILogger _logger;
        private GameScreen? _screen;

        private Stack<IGameScene> _scenes = new();
        private EnumMap<HighlightLayerName, ICompositeHighlight> _currentHighlights;

        public GameController(UiWindow window, World? world, ViewFactory viewFactory, ILogger logger)
        {
            _window = window;
            _world = world;
            _viewFactory = viewFactory;
            _logger = logger;

            _currentHighlights = new()
            {
                [HighlightLayerName.Background] = SimpleHighlight.Wrap(SubRegionHighlight.Create())
            };
            if (world != null)
            {
                _currentHighlights[HighlightLayerName.Midground] =
                    FactionHighlight.Create(world, viewFactory.BannerViewFactory);
            }
        }

        public void Bind(object @object)
        {
            _screen = @object as GameScreen;
            if (_screen!.EmpireOverlay.Controller is IOverlayController controller)
            {
                controller.ButtonClicked += HandleButton;
            }
        }

        public void Unbind()
        {
            _screen = null;
            if (_screen!.EmpireOverlay.Controller is IOverlayController controller)
            {
                controller.ButtonClicked -= HandleButton;
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
                ChangeSceneTo(_viewFactory.SceneFactory.Create(galaxy), cleanUp);
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
            if (_screen?.Scene?.Controller is ISceneController oldSceneController)
            {
                oldSceneController.Interacted -= HandleInteraction;
            }
            if (scene.Controller is ISceneController sceneController)
            {
                sceneController.Interacted += HandleInteraction;
            }
            foreach (var layer in _currentHighlights)
            {
                scene.SetHighlight(layer.Key, layer.Value);
            }
            _screen!.SetScene(scene);
            _window.SetFocus(scene);
        }

        public void HandleButton(object? sender, ElementEventArgs<OverlayButtonId> e)
        {
            _logger.AtInfo().Log(e.ToString());
            _screen!.ClearPanes();
            _screen!.OpenPane(GetPane(e.Element));
        }

        public void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log(e.ToString());
            if (e.Key == Keys.Backspace)
            {
                TryPopScene();
                return;
            }
            var @object = e.GetOnlyObject();
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
        
        private static GamePaneId GetPane(OverlayButtonId id)
        {
            return id switch
            {
                OverlayButtonId.Design => GamePaneId.Design,
                OverlayButtonId.Military => GamePaneId.Military,
                OverlayButtonId.Research => GamePaneId.Research,
                _ => GamePaneId.None,
            };
        }
    }
}
