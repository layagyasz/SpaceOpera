using SpaceOpera.Core.Ai.Diplomacy;
using SpaceOpera.Core.Events;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Ai.Events
{
    public class EventAi
    {
        public Faction Faction { get; }

        private readonly DiplomacyAi _diplomacy;

        public EventAi(Faction faction, DiplomacyAi diplomacy)
        {
            Faction = faction;
            _diplomacy = diplomacy;
        }

        public void Tick(World world)
        {
            foreach (var @event in world.Events.Get(Faction).ToList())
            {
                if (@event is NotificationBase)
                {
                    world.Execute(new DecideEventOrder(@event, /* decisionId= */ 0));
                }
                if (@event is ProposeDiplomaticAgreementEvent diplomacy)
                {
                    world.Execute(
                        new DecideEventOrder(@event, _diplomacy.GetApproval(diplomacy.Agreement).Result > 0 ? 0 : 1));
                }
            }
        }
    }
}
