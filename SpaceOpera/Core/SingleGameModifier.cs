using Cardamom.Json;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core
{
    public class SingleGameModifier
    {
        public ModifierType Type { get; set; }
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public IMaterial? Material { get; set; }
        public Modifier Modifier { get; set; }

        public static SingleGameModifier Create(ModifierType type, Modifier modifier)
        {
            return new SingleGameModifier()
            {
                Type = type,
                Modifier = modifier
            };
        }
    }
}
