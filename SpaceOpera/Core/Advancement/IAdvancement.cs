namespace SpaceOpera.Core.Advancement
{
    public interface IAdvancement
    {
        public AdvancementType? Type { get; }
        public float Cost { get; }
        public List<IAdvancement> Prerequisites { get; }
    }
}
