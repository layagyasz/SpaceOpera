using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core
{
    public class HumanPlayer : IPlayer
    {
        public Faction Faction { get; }

        public HumanPlayer(Faction faction)
        {
            Faction = faction;
        }

        public ModifiedResult? GetApproval(Faction faction)
        {
            return null;
        }
    }
}
