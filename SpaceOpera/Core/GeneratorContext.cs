using Cardamom.Logging;
using SpaceOpera.Core.Loader;
using SpaceOpera.Core.Universe.Generator;

namespace SpaceOpera.Core
{
    public class GeneratorContext
    {
        public ILogger? Logger { get; }
        public LoaderStatus? LoaderStatus { get; }
        public StellarBodySurfaceGeneratorResources? StellarBodySurfaceGeneratorResources { get; }
        public Random Random { get; }

        public GeneratorContext(
            ILogger? logger, 
            LoaderStatus? loaderStatus, 
            StellarBodySurfaceGeneratorResources? stellarBodySurfaceGeneratorResources,
            Random random)
        {
            Logger = logger;
            LoaderStatus = loaderStatus;
            StellarBodySurfaceGeneratorResources = stellarBodySurfaceGeneratorResources;
            Random = random;
        }
    }
}
