using Cardamom.Logging;
using Cardamom.Ui;
using SpaceOpera.Controller.Game;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe.Generator;
using SpaceOpera.View;

namespace SpaceOpera.Controller
{
    public class ProgramController
    {
        private readonly UiWindow _window;
        private readonly ILogger _logger;
        private readonly CoreData _coreData;
        private readonly ViewFactory _viewFactory;

        private IScreen? _screen;

        public ProgramController(UiWindow window, ILogger logger, CoreData coreData, ViewFactory viewFactory)
        {
            _window = window;
            _logger = logger;
            _coreData = coreData;
            _viewFactory = viewFactory;
        }

        public void EnterGame(World world, Faction playerFaction)
        {
            var driver = new GameDriver(world.GetUpdater());
            var controller = 
                new GameController(_window, world, driver, playerFaction, _viewFactory, _logger);
            ChangeScreen(_viewFactory.CreateGameScreen(controller));
            controller.PushScene(world.Galaxy);
            driver.Start();
        }

        public void EnterGameSetup()
        {
            ChangeScreen(_viewFactory.CreateGameSetupScreen(_coreData));
            var controller = (GameSetupController)_screen!.Controller;
            controller.GameStarted += HandleGameStarted;
        }

        public void Start()
        {
            _window.Start();
        }

        private void ChangeScreen(IScreen screen)
        {
            if (_screen != null)
            {
                DisposeScreen(_screen);
            }
            _window.SetRoot(screen);
            _screen = screen;
        }

        private void DisposeScreen(IScreen screen)
        {
            var controller = screen.Controller;
            if (controller is GameSetupController gameSetup)
            {
                gameSetup.GameStarted -= HandleGameStarted;
            }
            screen.Dispose();
        }

        private void HandleGameStarted(object? sender, GameParameters e)
        {
            var world =
                WorldGenerator.Generate(
                    e.WorldParameters,
                    e.PlayerCulture,
                    e.PlayerFaction,
                    _coreData,
                    new(_logger, StellarBodySurfaceGeneratorResources.CreateForGenerator(), new()));
            EnterGame(world, e.PlayerFaction);
        }
    }
}
