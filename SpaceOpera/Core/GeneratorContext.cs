using Cardamom.Logging;
using SpaceOpera.Core.Universe.Generator;

namespace SpaceOpera.Core
{
    public class GeneratorContext
    {
        public ILogger? Logger { get; }
        public StellarBodySurfaceGeneratorResources? StellarBodySurfaceGeneratorResources { get; }
        public Random Random { get; }

        public GeneratorContext(
            ILogger? logger, StellarBodySurfaceGeneratorResources? stellarBodySurfaceGeneratorResources, Random random)
        {
            Logger = logger;
            StellarBodySurfaceGeneratorResources = stellarBodySurfaceGeneratorResources;
            Random = random;
        }
    }
}
