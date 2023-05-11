using SpaceOpera.Core.Advancement;

namespace SpaceOpera.Core.Orders
{
    public class SetResearchOrder : IOrder
    {
        public FactionAdvancementManager AdvancementManager { get; }
        public AdvancementSlot Slot { get; }
        public IAdvancement Advancement { get; }

        public SetResearchOrder(
            FactionAdvancementManager advancementManager, AdvancementSlot slot, IAdvancement advancement)
        {
            AdvancementManager = advancementManager;
            Slot = slot;
            Advancement = advancement;
        }

        public ValidationFailureReason Validate()
        {
            if (AdvancementManager == null || Slot == null || Advancement == null)
            {
                return ValidationFailureReason.IllegalOrder;
            }
            if (!AdvancementManager.HasPrerequisiteResearch(Advancement))
            {
                return ValidationFailureReason.PrerequisiteResearch;
            }
            if (AdvancementManager.GetAdvancementSlots().Any(x => x.Advancement == Advancement))
            {
                return ValidationFailureReason.DuplicateResearch;
            }
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            Slot.Advancement = Advancement;
            return true;
        }
    }
}