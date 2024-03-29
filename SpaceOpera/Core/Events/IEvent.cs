﻿using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public interface IEvent
    {
        Faction Faction { get; }
        string GetTitle();
        string GetDescription();
        IEnumerable<EventDecision> GetDecisions();
        bool Decide(int decisionId, World world);
    }
}
