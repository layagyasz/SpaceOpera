namespace SpaceOpera.Core.Advancement
{
    public class AdvancementSlot
    {
        public int Id { get; set; }
        public IAdvancement? Advancement { get; set; }

        public AdvancementSlot(int id)
        {
            Id = id;
        }
    }
}