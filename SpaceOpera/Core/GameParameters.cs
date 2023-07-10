using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core
{
    public class GameParameters
    {
        public WorldGenerator.Parameters WorldParameters { get; }
        public Culture PlayerCulture { get; }
        public Faction PlayerFaction { get; }

        public GameParameters(WorldGenerator.Parameters worldParameters, Culture playerCulture, Faction playerFaction)
        {
            WorldParameters = worldParameters;
            PlayerCulture = playerCulture;
            PlayerFaction = playerFaction;
        }
    }
}
