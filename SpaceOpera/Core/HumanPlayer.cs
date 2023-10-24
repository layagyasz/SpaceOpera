using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

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

        public ModifiedResult? GetApproval(DiplomaticAgreement agreement)
        {
            return null;
        }

        public void Tick(World world) { }
    }
}
