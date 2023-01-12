namespace SpaceOpera.Core.Advancement
{
    public class BaseAdvancement : IAdvancement
    {
        public AdvancementType? Type { get; set; }
        public float Cost { get; set; }
        public List<IAdvancement> Prerequisites { get; set; } = new();
    }
}
