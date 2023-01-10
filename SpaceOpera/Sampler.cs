using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class Sampler
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum SamplerType
        {
            CONSTANT,
            EXPONENTIAL,
            GAMMA,
            NORMAL,
            PARETO,
            RECIPROCAL,
            UNIFORM
        }

        public SamplerType Type { get; set; }
        public double Scale { get; set; }
        public double Shape { get; set; }

        public Sampler() { }

        public Sampler(SamplerType Type, double Scale, double Shape)
        {
            this.Type = Type;
            this.Scale = Scale;
            this.Shape = Shape;
        }

        public double Sample(Random Random)
        {
            switch (Type)
            {
                case SamplerType.CONSTANT:
                    return Scale * Shape;
                case SamplerType.EXPONENTIAL:
                    return Scale * Exponential.Sample(Random, Shape);
                case SamplerType.GAMMA:
                    return Gamma.Sample(Random, Shape, Scale);
                case SamplerType.NORMAL:
                    return Normal.Sample(Random, Scale, Shape);
                case SamplerType.PARETO:
                    return Pareto.Sample(Random, Scale, Shape);
                case SamplerType.RECIPROCAL:
                    return Shape * Math.Pow(Scale, Random.NextDouble());
                case SamplerType.UNIFORM:
                    return Scale * Random.NextDouble() + Shape;
                default:
                    throw new InvalidOperationException(string.Format("Unsupported SamplerType: {0}", Type));
            }
        }
    }
}