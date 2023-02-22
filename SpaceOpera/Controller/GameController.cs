using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.View;
using SpaceOpera.View.Common.Highlights;
using SpaceOpera.View.Scenes;

namespace SpaceOpera.Controller
{
    public class GameController : IController
    {
        private readonly World? _world;
        private readonly ViewFactory _viewFactory;
        private readonly ILogger _logger;
        private GameScreen? _screen;

        private EnumMap<HighlightLayerName, ICompositeHighlight> _currentHighlights;

        public GameController(World? world, ViewFactory viewFactory, ILogger logger)
        {
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
        }

        public void ChangeScene(IGameScene scene)
        {
            scene.Initialize();
            if (scene.Controller is ISceneController sceneController)
            {
                sceneController.Interacted += (s, e) => HandleInteraction(e);
            }
            foreach (var layer in _currentHighlights)
            {
                scene.SetHighlight(layer.Key, layer.Value);
            }
            _screen!.SetScene(scene);

        }

        public void Unbind()
        {
            _screen = null;
        }

        public void HandleInteraction(UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log(e.ToString());
        }
    }
}
