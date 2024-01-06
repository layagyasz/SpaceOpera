using Cardamom;
using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Advancement
{
    public interface IAdvancement : IKeyed
    {
        public string Name { get; }
        public IMaterial? Type { get; }
        public float Cost { get; }
        public List<IAdvancement> Prerequisites { get; }
    }
}
