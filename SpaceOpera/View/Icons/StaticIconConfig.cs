using Cardamom;

namespace SpaceOpera.View.Icons
{
    public record class StaticIconConfig(string Key, List<IconLayer> Layers) : IKeyed
    {
        public string Key { get; set; } = Key;
    }
}
