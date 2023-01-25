using Cardamom;
using Cardamom.Json.Collections;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe
{
    public class Biome : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<GameModifier> Modifiers { get; set; } = new();
        public bool IsHabitable { get; set; }
        public bool IsTraversable { get; set; }
    }
}
