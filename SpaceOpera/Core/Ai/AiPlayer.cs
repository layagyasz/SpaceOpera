using SpaceOpera.Core.Ai.Diplomacy;
using SpaceOpera.Core.Ai.Events;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core.Ai
{
    public class AiPlayer : IPlayer
    {
        public Faction Faction { get; }

        private DiplomacyAi _diplomacy;
        private EventAi _event;

        public AiPlayer(Faction faction)
        {
            Faction = faction;

            _diplomacy = new(faction);
            _event = new(faction, _diplomacy);
        }

        public ModifiedResult? GetApproval(Faction faction)
        {
            return _diplomacy.GetApproval(faction);
        }

        public ModifiedResult? GetApproval(DiplomaticAgreement agreement)
        {
            return _diplomacy.GetApproval(agreement);
        }

        public void Tick(World world)
        {
            _event.Tick(world);
        }
    }
}
