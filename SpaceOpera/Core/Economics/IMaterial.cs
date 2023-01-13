using Cardamom;

namespace SpaceOpera.Core.Economics
{
    public interface IMaterial : IKeyed
    {
        string Name { get; }
        float Mass { get; }
        float Size { get; }
        MaterialType Type { get; }
    }
}
