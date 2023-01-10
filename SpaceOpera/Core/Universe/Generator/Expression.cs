using Cardamom.Spatial;
using Cence;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class Expression
    {
        private static readonly double NOISE_MEAN = .5;
        private static readonly double NOISE_STD_DEV = .075;

        public enum ExpressionType
        {
            PARAMETER,
            NOISE,
            RANDOM,
            CONSTANT
        }

        public enum ParameterName
        {
            WATER_LEVEL,
            PLANET_TEMPERATURE,
            PLANET_MOISTURE,
            HEIGHT,
            TEMPERATURE,
            MOISTURE
        }

        public enum ExpressionWaveForm
        {
            NONE,
            COSINE,
            SINE
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpressionType Type { get; set; }

        // Variable
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParameterName Parameter { get; set; }

        // Noise
        public Expression Bias { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpressionWaveForm WaveForm { get; set; }
        public Expression WaveAmplitude { get; set; }
        public Expression WavePeriodicity { get; set; }
        public Expression WaveTurbulence { get; set; }

        LatticeNoiseGenerator _Noise;

        // Random
        public Sampler Sampler { get; set; }

        float _Value;

        // Constant
        public float Constant { get; set; }

        public void Seed(Random Random)
        {
            switch (Type)
            {
                case ExpressionType.NOISE:
                    _Noise = new LatticeNoiseGenerator(Random) { Frequency = (x, y) => .0002 };
                    break;
                case ExpressionType.RANDOM:
                    _Value = (float)Sampler.Sample(Random);
                    break;
            }
        }

        public float Evaluate(float[] Parameters, Vector4f PositionCartesian, CSpherical PositionSpherical)
        {
            switch (Type)
            {
                case ExpressionType.PARAMETER:
                    return Parameters[(int)Parameter];
                case ExpressionType.NOISE:
                    return EvaluateNoise(Parameters, PositionCartesian, PositionSpherical);
                case ExpressionType.RANDOM:
                    return _Value;
                case ExpressionType.CONSTANT:
                    return Constant;
                default:
                    throw new NotSupportedException();
            }
        }

        private float EvaluateNoise(float[] Parameters, Vector4f PositionCartesian, CSpherical PositionSpherical)
        {
            float noiseValue =
                (float)Normal.CDF(
                    NOISE_MEAN,
                    NOISE_STD_DEV,
                    _Noise.Generate(
                        Math.Sqrt(
                            PositionCartesian.X * PositionCartesian.X + PositionCartesian.Y * PositionCartesian.Y)
                        * PositionSpherical.Phi,
                        PositionCartesian.Z));

            float bias = Bias?.Evaluate(Parameters, PositionCartesian, PositionSpherical) ?? 0;
            if (WaveForm == ExpressionWaveForm.NONE)
            {
                return noiseValue + bias;
            }

            float periodicity = WavePeriodicity?.Evaluate(Parameters, PositionCartesian, PositionSpherical) ?? 0;
            float turbulence = WaveTurbulence?.Evaluate(Parameters, PositionCartesian, PositionSpherical) ?? 0;
            float amplitude = WaveAmplitude?.Evaluate(Parameters, PositionCartesian, PositionSpherical) ?? 0;
            float wave = periodicity * (PositionSpherical.Theta + turbulence * (2 * noiseValue - 1));
            if (WaveForm == ExpressionWaveForm.COSINE)
            {
                return (float)(amplitude * .5f * Math.Cos(wave) + .5f) + bias;
            }
            return (float)(amplitude * .5f * Math.Sin(wave) + .5f) + bias;
        }
    }
}