using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Utils.Suppliers;
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
        private class GenerateWorldTask : ILoaderTask
        {
            public bool IsGL => true;
            public GameParameters Parameters { get; }
            public CoreData CoreData { get; }
            public GeneratorContext GeneratorContext { get; }

            private readonly Promise<World> _promise = new();

            public GenerateWorldTask(GameParameters parameters, CoreData coreData, GeneratorContext generatorContext)
            {
                Parameters = parameters;
                CoreData = coreData;
                GeneratorContext = generatorContext;
            }

            public Promise<World> GetPromise()
            {
                return _promise;
            }

            public bool IsDone()
            {
                return _promise.HasValue();
            }

            public void Perform()
            {
                var world = 
                    WorldGenerator.Generate(
                        Parameters.WorldParameters,
                        Parameters.PlayerFaction, 
                        CoreData, 
                        GeneratorContext);
                _promise.Set(world);
            }
        }

        private readonly UiWindow _window;
        private readonly Core.Loader.Loader _loaderThread;
        private readonly ILogger _logger;
        private readonly CoreData _coreData;
        private readonly ViewFactory _viewFactory;

        private IScreen? _screen;

        public ProgramController(
            UiWindow window, Core.Loader.Loader loaderThread, ILogger logger, CoreData coreData, ViewFactory viewFactory)
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
            var task = new GenerateWorldTask(parameters, _coreData, context);
            var screen = _viewFactory.CreateLoaderScreen(task, context.LoaderStatus!);
            var controller = (LoaderController)screen.Controller;
            controller.Finished += HandleWorldGenerated;
            _loaderThread.QueueTask(task);
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
