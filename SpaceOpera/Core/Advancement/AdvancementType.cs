using Cardamom;
using Cardamom.Json;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Advancement
{
    public class AdvancementType : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public IMaterial? Research { get; set; }
    }
}
