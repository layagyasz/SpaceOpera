using Cardamom.Logging;
using SpaceOpera.Core.Loader;
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

        public static LoaderTaskNode<World> Generate(
            Parameters parameters, Faction playerFaction, CoreData coreData, GeneratorContext context)
        {
            return coreData.GalaxyGenerator!.Generate(parameters.Galaxy, context).Map(
                galaxy =>
                {
                    var navigationMap = NavigationMap.Create(galaxy);
                    var world =
                        new World(
                            coreData,
                            context.Random,
                            galaxy,
                            new StarCalendar(/* startDate= */ 900000000),
                            navigationMap);
                    coreData.PoliticsGenerator!.Generate(parameters.Politics, world, playerFaction, context);
                    coreData.EconomyGenerator!.Generate(world, context);
                    return world;
                });
        }
    }
}
