using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public interface IEvent
    {
        Faction Faction { get; }
        string Title { get; }
        string Description { get; }
        IEnumerable<EventDecision> GetDecisions();
        bool Decide(int decisionId, World world);
    }
}
