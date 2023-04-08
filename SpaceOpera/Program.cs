using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe.Generator;
using SpaceOpera.View;

namespace SpaceOpera
{
    public class Program
    {
        enum RunMode
        {
            CompileSymbols,
            TestLanguage,
            TestStellarBody,
            TestSolarSystem,
            TestGalaxy,
            Full
        }

        static void Main()
        {
            RunMode mode = RunMode.Full;
            if (mode == RunMode.CompileSymbols)
            {
                int i = 0;
                var dir = "Resources/View/Patterns/";
                var prefix = "pattern";
                foreach (var file in Directory.GetFiles(".", "*.png"))
                {
                    var newName = $"{prefix}-{i++}.png";
                    File.Move(file, dir + newName);
                }
                return;
            }

            var monitor = Monitors.GetPrimaryMonitor();
            var window =
                new RenderWindow("SpaceOpera", new Vector2i(monitor.HorizontalResolution, monitor.VerticalResolution));
            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, new Keys[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }));

            ILogger logger = new Logger(new ConsoleBackend(), LogLevel.Info);
            var coreData = CoreData.LoadFrom("Resources/Core/CoreData.json", logger);
            var viewData = ViewData.LoadFrom("Resources/View/ViewData.json", logger);
            var viewFactory = ViewFactory.Create(viewData, coreData, logger);

            var generatorContext =
                new GeneratorContext(logger, StellarBodySurfaceGeneratorResources.CreateForGenerator(), new());
            if (mode == RunMode.TestLanguage)
            {
                var language = coreData.PoliticsGenerator!.Culture!.Language!.Generate(generatorContext);
                logger.AtInfo().Log(language.ToString());
                for (int i=0; i<20; ++i)
                {
                    logger.AtInfo().Log(language.GenerateWord(generatorContext.Random));
                }
                return;
            }

            var calendar = new StarCalendar(0);
            var playerCulture = coreData.PoliticsGenerator!.Culture!.Generate(generatorContext);
            var playerBanner = coreData.PoliticsGenerator!.Banner!.Generate(generatorContext);
            var playerFaction =
                coreData.PoliticsGenerator!.Faction!.Generate(playerCulture, playerBanner, generatorContext);
            object scene;
            GameController controller;
            GameDriver driver;
            if (mode == RunMode.TestStellarBody)
            {
                var planetGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .First(x => x.Generator!.Key == "planet-generator-cytherean").Generator!;
                var orbitGenerator = coreData.GalaxyGenerator!.StarSystemGenerator!.OrbitGenerator!;
                var starGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StarGenerators
                        .First(x => x.Key == "star-generator-class-g");
                var planet =
                    planetGenerator.Generate(
                        orbitGenerator.Generate(starGenerator.Generate(generatorContext), 1f, generatorContext), 
                        generatorContext);
                scene = planet;
                driver = new(null, calendar);
                controller = new GameController(ui, null, driver, playerFaction, viewFactory, logger);
            }
            else if (mode == RunMode.TestSolarSystem)
            {
                var system = coreData.GalaxyGenerator!.StarSystemGenerator!.Generate(new(), generatorContext);
                scene = system;
                driver = new(null, calendar);
                controller = new GameController(ui, null, driver, playerFaction,viewFactory, logger);
            }
            else if (mode == RunMode.TestGalaxy)
            {
                var galaxy = coreData.GalaxyGenerator!.Generate(generatorContext);
                scene = galaxy;
                driver = new(null, calendar);
                controller = new GameController(ui, null, driver, playerFaction, viewFactory, logger);
            }
            else if (mode == RunMode.Full)
            {
                var world = WorldGenerator.Generate(playerCulture, playerFaction, coreData, generatorContext);
                scene = world.Galaxy;
                driver = new(world, world.GetUpdater());
                controller = new GameController(ui, world, driver, playerFaction, viewFactory, logger);
                calendar = world.Calendar;
            }
            else
            {
                throw new ArgumentException();
            }
            var screen = viewFactory.CreateGameScreen(controller, calendar);
            driver.Start();
            ui.SetRoot(screen);
            controller.PushScene(scene);
            ui.Start();
        }
    }
}