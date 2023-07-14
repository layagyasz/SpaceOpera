using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core
{
    public class GameParameters
    {
        public WorldGenerator.Parameters WorldParameters { get; }
        public Faction PlayerFaction { get; }

        public GameParameters(WorldGenerator.Parameters worldParameters, Faction playerFaction)
        {
            WorldParameters = worldParameters;
            PlayerFaction = playerFaction;
        }
    }
}
