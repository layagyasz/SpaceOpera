using Cardamom.Json;
using Cardamom.Mathematics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StellarBodyGeneratorOption
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public StellarBodyGenerator? Generator { get; set; }
        public float Weight { get; set; }
        public Interval ThermalRange { get; set; } = Interval.Unbounded;
        public Interval GravitationalRange { get; set; } = Interval.Unbounded;

        public bool Satisfies(float temperature, float gravity)
        {
            return ThermalRange.Contains(temperature) && GravitationalRange.Contains(gravity);
        }
    }
}