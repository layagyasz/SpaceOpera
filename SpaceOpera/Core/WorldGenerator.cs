using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core
{
    public static class WorldGenerator
    {
        public static World Generate(
            Culture playerCulture, Faction playerFaction, CoreData coreData, GeneratorContext context)
        {
            var logger = context.Logger.ForType(typeof(WorldGenerator)).AtInfo();
            logger.Log("Generate galaxy");
            var galaxy = coreData.GalaxyGenerator!.Generate(context);
            logger.Log($"Generated galaxy with size {galaxy.GetSize()}");
            logger.Log("Build navigation map");
            var navigationMap = NavigationMap.Create(galaxy);
            logger.Log($"Built navigation map with size {navigationMap.GetSize()}");
            var world =
                new World(
                    coreData,
                    context.Random,
                    galaxy,
                    new StarCalendar(/* StartDate= */ 1440000),
                    navigationMap);
            logger.Log("Generate politics");
            coreData.PoliticsGenerator!.Generate(world, playerCulture, playerFaction, context);
            logger.Log("Generate economy");
            coreData.EconomyGenerator!.Generate(world, context);
            return world;
        }
    }
}
