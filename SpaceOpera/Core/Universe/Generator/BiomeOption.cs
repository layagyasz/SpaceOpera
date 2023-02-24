using Cardamom.Json;
using Cardamom.Mathematics;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class BiomeOption
    {
        [JsonConverter(typeof(ReferenceJsonConverter))]
        public Biome? Biome { get; set; }
        public Interval ThermalRange { get; set; } = Interval.Unbounded;
        public Vector4 Center { get; set; }
        public Vector4 AxisWeight { get; set; }
        public float Weight { get; set; } = 1;
        public float BlendRange { get; set; } = 1;
    }
}
