using Cardamom;

namespace SpaceOpera.Core.Universe
{
    public class Biome : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<GameModifier> Modifiers { get; set; } = new();
    }
}
