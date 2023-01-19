using Cardamom.Mathematics.Coordinates;
using Cardamom.Utils.Generators.Samplers;
using MathNet.Numerics.Distributions;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class Expression
    {
        private static readonly double s_NoiseMean = .5;
        private static readonly double s_NoiseStandardDeviation = .075;

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public enum ExpressionType
        {
            Parameter,
            Noise,
            Random,
            Constant
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public enum ParameterName
        {
            WaterLevel,
            PlanetTemperature,
            PlanetMoisture,
            Height,
            Temperature,
            Moisture
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]

        public enum ExpressionWaveForm
        {
            None,
            Cosine,
            Sine
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpressionType Type { get; set; }

        // Variable
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParameterName Parameter { get; set; }

        // Noise
        public Expression? Bias { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExpressionWaveForm WaveForm { get; set; }
        public Expression? WaveAmplitude { get; set; }
        public Expression? WavePeriodicity { get; set; }
        public Expression? WaveTurbulence { get; set; }

        LatticeNoiseGenerator _Noise;

        // Random
        public ISampler? Sampler { get; set; }

        float _Value;

        // Constant
        public float Constant { get; set; }

        public void Seed(Random random)
        {
            switch (Type)
            {
                case ExpressionType.Noise:
                    _Noise = new LatticeNoiseGenerator(random) { Frequency = (x, y) => .0002 };
                    break;
                case ExpressionType.Random:
                    _Value = Sampler!.Generate(random);
                    break;
            }
        }

        public float Evaluate(float[] parameters, Vector3 positionCartesian, Spherical3 positionSpherical)
        {
            switch (Type)
            {
                case ExpressionType.Parameter:
                    return parameters[(int)Parameter];
                case ExpressionType.Noise:
                    return EvaluateNoise(parameters, positionCartesian, positionSpherical);
                case ExpressionType.Random:
                    return _Value;
                case ExpressionType.Constant:
                    return Constant;
                default:
                    throw new NotSupportedException();
            }
        }

        private float EvaluateNoise(float[] parameters, Vector3 positionCartesian, Spherical3 positionSpherical)
        {
            float noiseValue =
                (float)Normal.CDF(
                    s_NoiseMean,
                    s_NoiseStandardDeviation,
                    _Noise.Generate(
                        Math.Sqrt(
                            positionCartesian.X * positionCartesian.X + positionCartesian.Y * positionCartesian.Y)
                        * positionSpherical.Zenith,
                        positionCartesian.Z));

            float bias = Bias?.Evaluate(parameters, positionCartesian, positionSpherical) ?? 0;
            if (WaveForm == ExpressionWaveForm.None)
            {
                return noiseValue + bias;
            }

            float periodicity = WavePeriodicity?.Evaluate(parameters, positionCartesian, positionSpherical) ?? 0;
            float turbulence = WaveTurbulence?.Evaluate(parameters, positionCartesian, positionSpherical) ?? 0;
            float amplitude = WaveAmplitude?.Evaluate(parameters, positionCartesian, positionSpherical) ?? 0;
            float wave = periodicity * (positionSpherical.Azimuth + turbulence * (2 * noiseValue - 1));
            if (WaveForm == ExpressionWaveForm.Cosine)
            {
                return (float)(amplitude * .5f * Math.Cos(wave) + .5f) + bias;
            }
            return (float)(amplitude * .5f * Math.Sin(wave) + .5f) + bias;
        }
    }
}