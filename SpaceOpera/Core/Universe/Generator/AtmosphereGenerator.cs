using SpaceOpera.Core.Economics;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class AtmosphereGenerator
    {
        public float RegionDensity { get; set; }
        public Sampler TotalPressureSampler { get; set; }
        [JsonConverter(typeof(KeyValuePairJsonConverter<Dictionary<IMaterial, Sampler>, IMaterial, Sampler>))]
        public Dictionary<IMaterial, Sampler> PartialPressureSamplers { get; set; }

        public MultiQuantity<IMaterial> Generate(Random Random)
        {
            var result = new MultiQuantity<IMaterial>();
            float pressure = (float)TotalPressureSampler.Sample(Random);
            foreach (var sampler in PartialPressureSamplers)
            {
                float weight = (float)sampler.Value.Sample(Random);
                float efficiency = Round(pressure * weight, 2);
                if (efficiency > 0)
                {
                    result.Add(sampler.Key, efficiency);
                }
            }
            return result;
        }

        private static float Round(float X, int Figures)
        {
            int offset = (int)Math.Log10(X);
            int m = (int)Math.Pow(10, Figures - offset);
            return (float)(Math.Round(m * X) / m);
        }
    }
}