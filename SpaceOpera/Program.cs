using Cardamom.Interface;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SpaceOpera.Controller;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.View;
using System;

namespace SpaceOpera
{
    class Program
    {
        static void Main(string[] args)
        {
            GameData gameData = GameData.FromPath("Resources");
            Random random = new();

            Culture culture = gameData.PoliticsGenerator.Culture!.Generate(random);
            Faction faction =
                gameData.PoliticsGenerator.Faction!.Generate(
                    culture, gameData.PoliticsGenerator.Banner!.Generate(random), random);
            World world = World.Generate(culture, faction, gameData, random);


            var renderWindow = new LitRenderWindow(VideoMode.DesktopMode, "Space Opera", Styles.Default);
            renderWindow.SetLightingShader(gameData.LightingShader);
            var game = new Interface(renderWindow);

            GameDriver driver = new GameDriver(world);
            driver.AddTickable(world.GetTickable());
            GameController gameController =
                new GameController(
                    new Vector2f(game.Window.Size.X, game.Window.Size.Y), game, world, driver, faction);
            gameController.ChangeScreenTo(world.Galaxy);
            gameController.Start();
        }
    }
}