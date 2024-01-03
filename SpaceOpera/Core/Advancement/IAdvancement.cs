using Cardamom;

namespace SpaceOpera.Core.Advancement
{
    public interface IAdvancement : IKeyed
    {
        public string Name { get; }
        public AdvancementType? Type { get; }
        public float Cost { get; }
        public List<IAdvancement> Prerequisites { get; }
    }
}
