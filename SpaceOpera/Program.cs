using Cardamom.Graphics.Ui;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
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

            var gameData = GameData.LoadFrom("Resources/GameData.json");
            var viewData = ViewData.LoadFrom("Resources/ViewData.json");
            var viewFactory = ViewFactory.Create(viewData, gameData);

            var random = new Random();
            var planetGenerator = 
                gameData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options.First().Generator!;
            var orbitGenerator = gameData.GalaxyGenerator!.StarSystemGenerator!.OrbitGenerator!;
            var starGenerator = gameData.GalaxyGenerator!.StarSystemGenerator!.StarGenerators.First()!;
            var planet = 
                planetGenerator.Generate(random, orbitGenerator.Generate(random, starGenerator.Generate(random), 0));
            var surface = viewFactory.StellarBodyViewFactory.GenerateSurfaceFor(planet);
            surface.GetTexture().CopyToImage().SaveToFile("planet-surface.png");
        }
    }
}