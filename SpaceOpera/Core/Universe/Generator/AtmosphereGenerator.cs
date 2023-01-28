using Cardamom.Json.Collections;
using Cardamom.Trackers;
using Cardamom.Utils.Generators.Samplers;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class AtmosphereGenerator
    {
        public float RegionDensity { get; set; }
        public ISampler? TotalPressureSampler { get; set; }
        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public Dictionary<IMaterial, ISampler> PartialPressureSamplers { get; set; } = new();

        public MultiQuantity<IMaterial> Generate(Random random)
        {
            var result = new MultiQuantity<IMaterial>();
            float pressure = (float)TotalPressureSampler!.Generate(random);
            foreach (var sampler in PartialPressureSamplers)
            {
                float weight = sampler.Value.Generate(random);
                float efficiency = Round(pressure * weight, 2);
                if (efficiency > 0)
                {
                    result.Add(sampler.Key, efficiency);
                }
            }
            return result;
        }

        private static float Round(float x, int figures)
        {
            int offset = (int)Math.Log10(x);
            int m = (int)Math.Pow(10, figures - offset);
            return (float)(Math.Round(m * x) / m);
        }
    }
}