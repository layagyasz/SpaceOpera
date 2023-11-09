using Cardamom.Json.Collections;
using Cardamom.Trackers;
using Cardamom.Utils.Generators.Samplers;
using SpaceOpera.Core.Economics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class AtmosphereGenerator
    {
        private static readonly float s_BaseOpticalThickness = 0.1f;
        private static readonly float s_BaseThickness = 300;
        private static readonly float s_BaseFalloff = 4f;

        public float RegionDensity { get; set; }
        public ISampler? TotalPressureSampler { get; set; }
        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public Dictionary<IMaterial, ISampler> PartialPressureSamplers { get; set; } = new();

        public Atmosphere Generate(float bodyMass, float bodyRadius, GeneratorContext context)
        {
            var random = context.Random;
            var result = new MultiQuantity<IMaterial>();
            float pressure = Math.Max(0, TotalPressureSampler!.Generate(random));
            foreach (var sampler in PartialPressureSamplers)
            {
                float weight = sampler.Value.Generate(random);
                float efficiency = Round(pressure * weight, 2);
                if (efficiency > 0)
                {
                    result.Add(sampler.Key, efficiency);
                }
            }

            float g = 
                MathF.Sqrt(
                    .000001f * Constants.GravitationalConstant * bodyMass 
                    / (bodyRadius * bodyRadius * Constants.EarthGravity));
            float falloff = s_BaseFalloff * g;
            return new(
                result,
                bodyRadius + s_BaseThickness / MathF.Sqrt(g), 
                -pressure * falloff * falloff * s_BaseOpticalThickness / (1 - falloff - MathF.Exp(-falloff)),
                falloff);
        }

        private static float Round(float x, int figures)
        {
            int offset = (int)Math.Log10(x);
            int m = (int)Math.Pow(10, figures - offset);
            return (float)(Math.Round(m * x) / m);
        }
    }
}