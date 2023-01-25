using Cardamom.Json.Collections;
using Cardamom.Trackers;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Economics
{
    public class SingleMaterialSink
    {
        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public MultiQuantity<IMaterial> Materials { get; set; } = new();
    }
}
