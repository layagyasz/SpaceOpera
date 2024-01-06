using Cardamom.Json;
using Cardamom.Json.Collections;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Advancement
{
    public class BaseAdvancement : IAdvancement
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public IMaterial? Type { get; set; }
        public float Cost { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<IAdvancement> Prerequisites { get; set; } = new();

        public override string ToString()
        {
            return $"[Advancement: Key={Key}]";
        }
    }
}
