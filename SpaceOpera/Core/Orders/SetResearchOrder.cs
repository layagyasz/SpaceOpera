using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Politics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class SetResearchOrder : IImmediateOrder
    {
        public Faction Faction { get; }
        public AdvancementSlot AdvancementSlot { get; }
        public IAdvancement Advancement { get; }

        public SetResearchOrder(Faction Faction, AdvancementSlot AdvancementSlot, IAdvancement Advancement)
        {
            this.Faction = Faction;
            this.AdvancementSlot = AdvancementSlot;
            this.Advancement = Advancement;
        }

        public ValidationFailureReason Validate()
        {
            if (Faction == null || AdvancementSlot == null || Advancement == null)
            {
                return ValidationFailureReason.ILLEGAL_ORDER;
            }
            if (!Faction.HasPrerequisiteResearch(Advancement))
            {
                return ValidationFailureReason.PREREQUISITE_RESEARCH;
            }
            if (Faction.GetAdvancementSlots().Any(x => x.Advancement == Advancement))
            {
                return ValidationFailureReason.DUPLICATE_RESEARCH;
            }
            return ValidationFailureReason.NONE;
        }

        public bool Execute(World World)
        {
            AdvancementSlot.Advancement = Advancement;
            return true;
        }
    }
}