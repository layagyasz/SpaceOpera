namespace SpaceOpera.Core.Universe.Generator
{
    public class BiomeOption
    {
        public Biome? Biome { get; set; }
        public List<BiomeCondition> Conditions { get; set; } = new();
    }
}
