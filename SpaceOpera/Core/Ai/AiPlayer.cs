using SpaceOpera.Core.Ai.Diplomacy;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Ai
{
    public class AiPlayer : IPlayer
    {
        public Faction Faction { get; }

        private DiplomacyAi _diplomacy;

        public AiPlayer(Faction faction)
        {
            Faction = faction;

            _diplomacy = new(faction);
        }

        public ModifiedResult? GetApproval(Faction faction)
        {
            return _diplomacy.GetApproval(faction);
        }
    }
}
