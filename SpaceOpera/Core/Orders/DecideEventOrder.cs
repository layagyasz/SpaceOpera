using SpaceOpera.Core.Events;

namespace SpaceOpera.Core.Orders
{
    public class DecideEventOrder : IOrder
    {
        public IEvent Event { get; }
        public int DecisionId { get; }

        public DecideEventOrder(IEvent @event, int decisionId) 
        {
            Event = @event;
            DecisionId = decisionId;
        }

        public ValidationFailureReason Validate(World world)
        {
            return Event.GetDecisions().Any(x => x.Id == DecisionId) 
                ? ValidationFailureReason.None 
                : ValidationFailureReason.IllegalOrder;
        }

        public bool Execute(World world)
        {
            world.Events.Decide(Event, DecisionId, world);
            return true;
        }
    }
}
