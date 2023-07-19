using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Governments;

namespace SpaceOpera.Controller.GameSetup
{
    public class GovernmentParameters
    {
        public string Name { get; }
        public NameGenerator NameGenerator { get; }
        public GovernmentForm? Government { get; }

        public GovernmentParameters(string name, NameGenerator nameGenerator, GovernmentForm? government)
        {
            Name = name;
            NameGenerator = nameGenerator;
            Government = government;
        }
    }
}
