using SpaceOpera.Core.Politics;

namespace SpaceOpera.Controller.GameSetup
{
    public class GovernmentParameters
    {
        public string Name { get; }
        public NameGenerator NameGenerator { get; }

        private GovernmentParameters(string name, NameGenerator nameGenerator)
        {
            Name = name;
            NameGenerator = nameGenerator;
        }

        public static GovernmentParameters Create(string name, NameGenerator nameGenerator)
        {
            return new(name, nameGenerator);
        }
    }
}
