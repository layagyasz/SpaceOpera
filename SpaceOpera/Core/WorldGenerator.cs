using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;

namespace SpaceOpera.Core
{
    public static class WorldGenerator
    {
        public struct Parameters
        {
            public GalaxyGenerator.Parameters Galaxy { get; set; }
            public PoliticsGenerator.Parameters Politics { get; set; }
        }

        public static World Generate(
            Parameters parameters, 
            Culture playerCulture, 
            Faction playerFaction, 
            CoreData coreData, 
            GeneratorContext context)
        {
            var logger = context.Logger.ForType(typeof(WorldGenerator)).AtInfo();
            logger.Log("Generate galaxy");
            var galaxy = coreData.GalaxyGenerator!.Generate(parameters.Galaxy, context);
            logger.Log($"Generated galaxy with size {galaxy.GetSize()}");
            logger.Log("Build navigation map");
            var navigationMap = NavigationMap.Create(galaxy);
            logger.Log($"Built navigation map with size {navigationMap.GetSize()}");
            var world =
                new World(
                    coreData,
                    context.Random,
                    galaxy,
                    new StarCalendar(/* startDate= */ 900000000),
                    navigationMap);
            logger.Log("Generate politics");
            coreData.PoliticsGenerator!.Generate(parameters.Politics, world, playerCulture, playerFaction, context);
            logger.Log("Generate economy");
            coreData.EconomyGenerator!.Generate(world, context);
            return world;
        }
    }
}
