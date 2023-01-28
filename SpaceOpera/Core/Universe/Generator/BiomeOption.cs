using Cardamom.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class BiomeOption
    {
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public Biome? Biome { get; set; }
        public List<BiomeCondition> Conditions { get; set; } = new();
    }
}
