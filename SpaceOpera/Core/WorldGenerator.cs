using Cardamom.Logging;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;

namespace SpaceOpera.Core
{
    public static class WorldGenerator
    {
        public enum Step
        {
            Galaxy,
            Culture,
            States,
            Resources,
            Economy
        }

        public struct Parameters
        {
            public GalaxyGenerator.Parameters Galaxy { get; set; }
            public PoliticsGenerator.Parameters Politics { get; set; }
        }

        public static GeneratorContext CreateContext(ILogger logger)
        {
            return new(
                logger, 
                new(Enum.GetValues<Step>().Cast<object>(), /* logLength= */ 10),
                StellarBodySurfaceGeneratorResources.CreateForGenerator(), 
                new());
        }

        public static World Generate(
            Parameters parameters, 
            Culture playerCulture, 
            Faction playerFaction, 
            CoreData coreData, 
            GeneratorContext context)
        {
            var galaxy = coreData.GalaxyGenerator!.Generate(parameters.Galaxy, context);
            var navigationMap = NavigationMap.Create(galaxy);
            var world =
                new World(
                    coreData,
                    context.Random,
                    galaxy,
                    new StarCalendar(/* startDate= */ 900000000),
                    navigationMap);
            coreData.PoliticsGenerator!.Generate(parameters.Politics, world, playerCulture, playerFaction, context);
            coreData.EconomyGenerator!.Generate(world, context);
            return world;
        }
    }
}
