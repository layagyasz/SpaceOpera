using MathNet.Numerics.Distributions;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Economics.Generator
{
    public class ResourceSampler
    {
        private static readonly double s_ClumpingConstant = .05;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ResourceSamplerType
        {
            Clumped,
            Dispersed,
            Ubiquitous
        }

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
            double x;
            switch (SamplerType)
            {
                case ResourceSamplerType.Clumped:
                    x = Pareto.Sample(random, .5 * NodeDensityCoefficient / Clumping(), 2);
                    break;
                case ResourceSamplerType.Dispersed:
                    x = Normal.Sample(random, NodeDensityCoefficient, .25 * NodeDensityCoefficient);
                    break;
                case ResourceSamplerType.Ubiquitous:
                default:
                    x = 0;
                    break;
            }
            x *= scale;
            return new ResourceNode(
                Resource,
                (int)x + Bernoulli.Sample(random, x - (int)Math.Floor(x)));
        }

        private double Clumping()
        {
            if (SamplerType == ResourceSamplerType.Clumped)
            {
                return s_ClumpingConstant;
            }
            return 1;
        }
    }
}