using Cardamom.Logging;
using Cardamom.Ui;
using SpaceOpera.Controller.Game;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Controller.Loader;
using SpaceOpera.Core;
using SpaceOpera.Core.Loader;
using SpaceOpera.Core.Politics;
using SpaceOpera.View;

namespace SpaceOpera.Controller
{
    public class ProgramController
    {
        private class GenerateWorldTask : MapLoaderTask<World, World>
        {
            public GameParameters Parameters { get; }

            public GenerateWorldTask(GameParameters parameters, LoaderTaskNode<World> generator)
                : base(generator, x => x, /* isGL= */ false)
            {
                Parameters = parameters;
                generator.AddChild(this);
            }
        }

        private readonly UiWindow _window;
        private readonly ThreadedLoader _loaderThread;
        private readonly ILogger _logger;
        private readonly CoreData _coreData;
        private readonly ViewFactory _viewFactory;

        private IScreen? _screen;

        public ProgramController(
            UiWindow window, ThreadedLoader loaderThread, ILogger logger, CoreData coreData, ViewFactory viewFactory)
        {
            _window = window;
            _loaderThread = loaderThread;
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

        private void GenerateWorld(GameParameters parameters)
        {
            var context = WorldGenerator.CreateContext(_logger);
            var task =
                new GenerateWorldTask(
                    parameters,
                    WorldGenerator.Generate(
                        parameters.WorldParameters,
                        parameters.PlayerFaction,
                        _coreData,
                        context));
            var screen = _viewFactory.CreateLoaderScreen(task, context.LoaderStatus!);
            var controller = (LoaderController)screen.Controller;
            controller.Finished += HandleWorldGenerated;
            _loaderThread.QueueTaskTree(task);
            ChangeScreen(screen);
        }

        private void HandleGameStarted(object? sender, GameParameters e)
        {
            GenerateWorld(e);
        }

        private void HandleWorldGenerated(object? sender, EventArgs e)
        {
            var controller = (LoaderController)sender!;
            controller.Finished -= HandleWorldGenerated;
            var task = (GenerateWorldTask)controller.Task;
            EnterGame(task.GetPromise().Get(), task.Parameters.PlayerFaction);
        }
    }
}
