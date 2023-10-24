using SpaceOpera.Core.Ai;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core
{
    public class PlayerManager
    {
        private readonly Dictionary<Faction, IPlayer> _players = new();

        public void Add(Faction faction, bool isHuman)
        {
            _players.Add(faction, isHuman ? new HumanPlayer(faction) : new AiPlayer(faction));
        }

        public IPlayer Get(Faction faction)
        {
            return _players[faction];
        }

        public void Tick(World world)
        {
            foreach (var player in _players.Values)
            {
                player.Tick(world);
            }
        }
    }
}
