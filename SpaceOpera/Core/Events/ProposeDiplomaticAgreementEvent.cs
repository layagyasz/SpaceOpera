﻿using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core.Events
{
    public class ProposeDiplomaticAgreementEvent : IEvent
    {
        private static readonly List<EventDecision> s_Decisions = new()
        {
            new(0, "Accept"),
            new(1, "Reject")
        };

        public Faction Faction => Agreement.Approver;
        public DiplomaticAgreement Agreement { get; }

        public ProposeDiplomaticAgreementEvent(DiplomaticAgreement agreement)
        {
            Agreement = agreement;
        }

        public string GetTitle()
        {
            return "Diplomatic Proposal";
        }

        public string GetDescription()
        {
            return "[Placeholder] Diplomatic agreement proposed";
        }

        public IEnumerable<EventDecision> GetDecisions()
        {
            return s_Decisions;
        }

        public bool Decide(int decisionId, World world)
        {
            if (decisionId == 0)
            {
                world.DiplomaticRelations.Apply(world, Agreement);
                return true;
            }
            else if (decisionId == 1)
            {
                return true;
            }
            return false;
        }
    }
}
