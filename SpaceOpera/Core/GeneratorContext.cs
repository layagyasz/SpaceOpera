using Cardamom.Logging;

namespace SpaceOpera.Core
{
    public class GeneratorContext
    {
        public ILogger Logger { get; }
        public Random Random { get; }

        public GeneratorContext(ILogger logger, Random random)
        {
            Logger = logger;
            Random = random;
        }
    }
}
