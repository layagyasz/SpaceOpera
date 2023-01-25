using Cardamom.Json;
using MathNet.Numerics.Distributions;
using SpaceOpera.Core.Universe;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Economics.Generator
{
    public class ResourceSampler
    {
        private static readonly float s_ClumpingConstant = .05f;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ResourceSamplerType
        {
            Clumped,
            Dispersed,
            Ubiquitous
        }

        [JsonConverter(typeof(ReferenceJsonConverter))]
        public IMaterial? Resource { get; set; }
        
        public ResourceSamplerType SamplerType { get; set; }
        public float NodeDensityCoefficient { get; set; }

        public bool Appears(Random random)
        {
            if (SamplerType == ResourceSamplerType.Clumped)
            {
                return random.NextDouble() < Clumping();
            }
            return true;
        }

        public ResourceNode Generate(float scale, Random random)
        {
            var x = SamplerType switch
            {
                ResourceSamplerType.Clumped => Pareto.Sample(random, .5 * NodeDensityCoefficient / Clumping(), 2),
                ResourceSamplerType.Dispersed => 
                    Normal.Sample(random, NodeDensityCoefficient, .25 * NodeDensityCoefficient),
                _ => 0,
            };
            x *= scale;
            return new ResourceNode(
                Resource!,
                (int)x + Bernoulli.Sample(random, x - (int)Math.Floor(x)));
        }

        private float Clumping()
        {
            if (SamplerType == ResourceSamplerType.Clumped)
            {
                return s_ClumpingConstant;
            }
            return 1;
        }
    }
}