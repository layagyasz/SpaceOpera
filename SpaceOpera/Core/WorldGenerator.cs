using Cardamom.Json;
using SpaceOpera.Core.Economics.Generator;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core
{
    public static class WorldGenerator
    {
        public static World Generate(Culture playerCulture, Faction playerFaction, CoreData coreData, Random random)
        {
            var galaxy = coreData.GalaxyGenerator!.Generate(random);
            var world =
                new World(
                    coreData,
                    random,
                    galaxy,
                    new StarCalendar(/* StartDate= */ 1440000),
                    NavigationMap.Create(galaxy));
            coreData.PoliticsGenerator!.Generate(world, playerCulture, playerFaction, random);
            coreData.EconomyGenerator!.Generate(world, random);
            return world;
        }
    }
}
