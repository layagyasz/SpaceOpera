using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core;
using SpaceOpera.Views;

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

            var coreData = CoreData.LoadFrom("Resources/Core/CoreData.json");
            var viewData = ViewData.LoadFrom("Resources/View/ViewData.json");
            var viewFactory = ViewFactory.Create(viewData, coreData);

            var random = new Random();
            IScene scene;
            int mode = 4;
            if (mode == 1)
            {
                var planetGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .First(x => x.Generator!.Key == "planet-generator-gaia").Generator!;
                var orbitGenerator = coreData.GalaxyGenerator!.StarSystemGenerator!.OrbitGenerator!;
                var starGenerator =
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StarGenerators
                        .First(x => x.Key == "star-generator-class-g");
                var planet =
                    planetGenerator.Generate(
                        random, orbitGenerator.Generate(random, starGenerator.Generate(random), 1f));
                scene = viewFactory.SceneFactory.Create(planet);
            }
            else if (mode == 2)
            {
                throw new ArgumentException("Star system view not yet implemented");
            }
            else if (mode == 3)
            {
                var galaxy = coreData.GalaxyGenerator!.Generate(random);
                scene = viewFactory.SceneFactory.Create(galaxy);
            }
            else if (mode == 4)
            {
                var playerCulture = coreData.PoliticsGenerator!.Culture!.Generate(random);
                var playerBanner = coreData.PoliticsGenerator!.Banner!.Generate(random);
                var playerFaction = coreData.PoliticsGenerator!.Faction!.Generate(playerCulture, playerBanner, random);
                var world = WorldGenerator.Generate(playerCulture, playerFaction, coreData, random);
                scene = viewFactory.SceneFactory.Create(world.Galaxy);
            }
            else
            {
                throw new ArgumentException();
            }

            var screen = new SceneScreen(new NoOpController<Screen>(), Enumerable.Empty<IUiLayer>(), scene);
            ui.SetRoot(screen);
            ui.Start();
        }
    }
}