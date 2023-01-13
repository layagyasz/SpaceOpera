namespace SpaceOpera.Core.Advancement
{
    public class AdvancementSlot
    {
        public int SlotId { get; set; }
        public IAdvancement? Advancement { get; set; }

        public AdvancementSlot(int slotId)
        {
            SlotId = slotId;
        }
    }
}