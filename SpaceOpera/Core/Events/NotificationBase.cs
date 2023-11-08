using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public abstract class NotificationBase : IEvent
    {
        private static readonly List<EventDecision> s_Decisions = new()
        {
            new(0, "Acknowledge")
        };

        public abstract Faction Faction { get; }

        public abstract string GetTitle();
        public abstract string GetDescription();

        public IEnumerable<EventDecision> GetDecisions()
        {
            return s_Decisions;
        }

        public bool Decide(int decisionId, World world)
        {
            return true;
        }
    }
}
