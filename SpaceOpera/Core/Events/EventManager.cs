using Cardamom.Collections;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public class EventManager
    {
        private readonly MultiMap<Faction, IEvent> _events = new();

        public void AddEvent(IEvent @event)
        {
            _events.Add(@event.Faction, @event);
        }

        public void Decide(IEvent @event, int decisionId, World world)
        {
            @event.Decide(decisionId, world);
            _events.Remove(@event.Faction, @event);
        }
    }
}
