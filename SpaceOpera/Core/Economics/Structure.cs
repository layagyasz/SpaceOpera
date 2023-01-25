using Cardamom;
using Cardamom.Json.Collections;
using Cardamom.Trackers;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Economics
{
    public class Structure : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public uint MaxWorkers { get; set; }
        public float BuildTime { get; set; }
        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public MultiQuantity<IMaterial> Cost { get; set; } = new();

        public override string ToString()
        {
            return string.Format("[Structure: Key={0}]", Key);
        }
    }
}