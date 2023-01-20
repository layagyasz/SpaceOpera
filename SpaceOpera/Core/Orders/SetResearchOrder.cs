using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Orders
{
    public class SetResearchOrder : IOrder
    {
        public Faction Faction { get; }
        public AdvancementSlot AdvancementSlot { get; }
        public IAdvancement Advancement { get; }

        public SetResearchOrder(Faction faction, AdvancementSlot advancementSlot, IAdvancement advancement)
        {
            Faction = faction;
            AdvancementSlot = advancementSlot;
            Advancement = advancement;
        }

        public ValidationFailureReason Validate()
        {
            if (Faction == null || AdvancementSlot == null || Advancement == null)
            {
                return ValidationFailureReason.IllegalOrder;
            }
            if (!Faction.HasPrerequisiteResearch(Advancement))
            {
                return ValidationFailureReason.PrerequisiteResearch;
            }
            if (Faction.GetAdvancementSlots().Any(x => x.Advancement == Advancement))
            {
                return ValidationFailureReason.DuplicateResearch;
            }
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            AdvancementSlot.Advancement = Advancement;
            return true;
        }
    }
}