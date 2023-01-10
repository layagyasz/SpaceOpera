using SFML.System;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    class StarSystemGenerator
    {
        [JsonConverter(typeof(WeightedVectorJsonConverter<StarGenerator>))]
        public WeightedVector<StarGenerator> StarGenerator { get; set; }
        public float TransitLimit { get; set; }
        public float StellarBodyDensity { get; set; }
        public Range ViableThermalRange { get; set; }
        public Range ViableGravitationalRange { get; set; }
        public OrbitGenerator OrbitGenerator { get; set; }
        public StellarBodyGeneratorSelector StellarBodySelector { get; set; }

        public StarSystem Generate(Random Random, Vector2f Position)
        {
            var star = StarGenerator[Random.NextDouble()].Generate(Random);
            float innerBoundary = 
                Math.Max(
                    GetDistanceForTemperature(star, ViableThermalRange.Maximum),
                    GetDistanceForGravity(star, ViableGravitationalRange.Maximum));
            float outerBoundary =
                Math.Max(
                    GetDistanceForTemperature(star, ViableThermalRange.Minimum), 
                    GetDistanceForGravity(star, ViableGravitationalRange.Minimum));
            float transitLimit = GetDistanceForGravity(star, TransitLimit);
            if (innerBoundary > outerBoundary)
            {
                return new StarSystem(
                    Position, star, float.NaN, outerBoundary, transitLimit, Enumerable.Empty<StellarBody>());
            }

            float numBodiesMean = StellarBodyDensity * (outerBoundary - innerBoundary);
            int numBodies =
                (int)Math.Round(
                    new Sampler(Sampler.SamplerType.NORMAL, numBodiesMean, numBodiesMean / 2).Sample(Random));
            var distanceSampler =
                new Sampler(Sampler.SamplerType.RECIPROCAL, outerBoundary / innerBoundary, innerBoundary);
            var orbiters = new List<StellarBody>();
            for (int i=0; i<numBodies;++i)
            {
                var orbit = OrbitGenerator.Generate(Random, star, (float)distanceSampler.Sample(Random));
                var avgDistance = orbit.GetAverageDistance();
                var generator = StellarBodySelector.Select(
                        Random,
                        GetTemperatureForDistance(star, avgDistance),
                        GetGravityForDistance(star, avgDistance));
                var stellarBody = generator.Generate(Random, orbit);
                orbiters.Add(stellarBody);
            }
            orbiters.Sort((x, y) => x.Orbit.GetAverageDistance().CompareTo(y.Orbit.GetAverageDistance()));
            return new StarSystem(Position, star, innerBoundary, outerBoundary, transitLimit, orbiters);
        }

        private float GetDistanceForTemperature(Star Star, float Temperature)
        {
            return Constants.ASTRAL_UNIT * (9 * Star.Temperature * Star.Temperature * Star.Radius) 
                / (16 * Temperature * Temperature);
        }

        private float GetTemperatureForDistance(Star Star, float Distance)
        {
            return (float)((3 * Star.Temperature * Math.Sqrt(Star.Radius)) 
                / (4 * Math.Sqrt(Distance / Constants.ASTRAL_UNIT)));
        }

        private float GetDistanceForGravity(Star Star, float Gravity)
        {
            return Constants.ASTRAL_UNIT * (float)Math.Sqrt(Constants.GRAVITATIONAL_CONSTANT * Star.Mass / Gravity);
        }

        private float GetGravityForDistance(Star Star, float Distance)
        {
            return (Constants.ASTRAL_UNIT * Constants.ASTRAL_UNIT * Constants.GRAVITATIONAL_CONSTANT * Star.Mass)
                / (Distance * Distance);
        }
    }
}