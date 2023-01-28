using Cardamom.Graphics.Ui;
using Cardamom.Utils.Suppliers;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Politics;

namespace SpaceOpera
{
    class Program
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

            Console.WriteLine(typeof(ConstantSupplier<Vector4>).AssemblyQualifiedName);

            GameData gameData = GameData.LoadFrom("Resources/GameData.json");
            Random random = new();

            Culture culture = gameData.PoliticsGenerator!.Culture!.Generate(random);
            Faction faction =
                gameData.PoliticsGenerator.Faction!.Generate(
                    culture, gameData.PoliticsGenerator.Banner!.Generate(random), random);
            World world = World.Generate(culture, faction, gameData, random);

            GameDriver driver = new(world);
            driver.Start();
            ui.Start();
        }
    }
}