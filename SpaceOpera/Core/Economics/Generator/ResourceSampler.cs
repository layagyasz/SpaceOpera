using MathNet.Numerics.Distributions;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics.Generator
{
    class ResourceSampler
    {
        private static readonly double CLUMPING_CONSTANT = .05;

        public enum ResourceSamplerType
        {
            CLUMPED,
            DISPERSED,
            UBIQUITOUS
        }

        public IMaterial Resource { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ResourceSamplerType SamplerType { get; set; }
        public float NodeDensityCoefficient { get; set; }

        public bool Appears(Random Random)
        {
            if (SamplerType == ResourceSamplerType.CLUMPED)
            {
                return Random.NextDouble() < Clumping();
            }
            return true;
        }

        public ResourceNode Generate(float Scale, Random Random)
        {
            double x;
            switch (SamplerType)
            {
                case ResourceSamplerType.CLUMPED:
                    x = Pareto.Sample(Random, .5 * NodeDensityCoefficient / Clumping(), 2);
                    break;
                case ResourceSamplerType.DISPERSED:
                    x = Normal.Sample(Random, NodeDensityCoefficient, .25 * NodeDensityCoefficient);
                    break;
                case ResourceSamplerType.UBIQUITOUS:
                default:
                    x = 0;
                    break;
            }
            x *= Scale;
            return new ResourceNode(
                Resource,
                (int)x + Bernoulli.Sample(Random, x - (int)Math.Floor(x)));
        }

        private double Clumping()
        {
            if (SamplerType == ResourceSamplerType.CLUMPED)
            {
                return CLUMPING_CONSTANT;
            }
            return 1;
        }
    }
}