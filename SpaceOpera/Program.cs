using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller;
using SpaceOpera.Controller.Scenes;
using SpaceOpera.Core;
using SpaceOpera.View;
using SpaceOpera.View.Scenes;

namespace SpaceOpera
{
    public class Program
    {
        static void Main()
        {
            var monitor = Monitors.GetPrimaryMonitor();
            var window =
                new RenderWindow("SpaceOpera", new Vector2i(monitor.HorizontalResolution, monitor.VerticalResolution));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            var logger = new Logger(new ConsoleBackend(), LogLevel.Info);
            var coreData = CoreData.LoadFrom("Resources/Core/CoreData.json", logger);
            var viewData = ViewData.LoadFrom("Resources/View/ViewData.json");
            var viewFactory = ViewFactory.Create(viewData, coreData);

            var generatorContext = new GeneratorContext(logger, new());
            IGameScene scene;
            GameController controller;
            int mode = 4;
            if (mode == 1)
            {
                var planetGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .First(x => x.Generator!.Key == "planet-generator-terran").Generator!;
                var orbitGenerator = coreData.GalaxyGenerator!.StarSystemGenerator!.OrbitGenerator!;
                var starGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StarGenerators
                        .First(x => x.Key == "star-generator-class-g");
                var planet =
                    planetGenerator.Generate(
                        orbitGenerator.Generate(starGenerator.Generate(generatorContext), 1f, generatorContext), 
                        generatorContext);
                scene = viewFactory.SceneFactory.Create(planet);
                controller = new GameController(null, viewFactory, logger);
            }
            else if (mode == 2)
            {
                throw new ArgumentException("Star system view not yet implemented");
            }
            else if (mode == 3)
            {
                var galaxy = coreData.GalaxyGenerator!.Generate(generatorContext);
                scene = viewFactory.SceneFactory.Create(galaxy);
                controller = new GameController(null, viewFactory, logger);
            }
            else if (mode == 4)
            {
                var playerCulture = coreData.PoliticsGenerator!.Culture!.Generate(generatorContext);
                var playerBanner = coreData.PoliticsGenerator!.Banner!.Generate(generatorContext);
                var playerFaction =
                    coreData.PoliticsGenerator!.Faction!.Generate(playerCulture, playerBanner, generatorContext);
                var world = WorldGenerator.Generate(playerCulture, playerFaction, coreData, generatorContext);
                scene = viewFactory.SceneFactory.Create(world.Galaxy);
                controller = new GameController(world, viewFactory, logger);
            }
            else
            {
                throw new ArgumentException();
            }
            var screen = new GameScreen(controller, Enumerable.Empty<IUiLayer>());
            screen.Initialize();
            controller.ChangeScene(scene);
            ui.SetRoot(screen);
            ui.Start();
        }
    }
}