using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Views;
using static System.Formats.Asn1.AsnWriter;
using System.Numerics;

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

            var gameData = GameData.LoadFrom("Resources/GameData.json");
            var viewData = ViewData.LoadFrom("Resources/ViewData.json");
            var viewFactory = ViewFactory.Create(viewData, gameData);

            var random = new Random();
            IScene scene;
            int mode = 1;
            if (mode == 1)
            {
                var planetGenerator =
                    gameData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .First(x => x.Generator!.Key == "planet-generator-gaia").Generator!;
                var orbitGenerator = gameData.GalaxyGenerator!.StarSystemGenerator!.OrbitGenerator!;
                var starGenerator =
                    gameData.GalaxyGenerator!.StarSystemGenerator!.StarGeneratorSelector.Get(random.NextSingle());
                var planet =
                    planetGenerator.Generate(
                        random, orbitGenerator.Generate(random, starGenerator.Generate(random), 80));
                scene = viewFactory.SceneFactory.Create(planet);
            }
            else if (mode == 2)
            {
                var galaxy = gameData.GalaxyGenerator!.Generate(random);
                scene = viewFactory.SceneFactory.Create(galaxy);
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