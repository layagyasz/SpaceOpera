using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Mathematics;
using Cardamom.Utils.Generators.Samplers;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StarSystemGenerator
    {
        private static Interval s_GoldilocksThermalRange = new(273f, 373f);

        [JsonConverter(typeof(FromFileJsonConverter))]
        public List<StarGenerator> StarGenerators { get; set; } = new();
        [JsonConverter(typeof(ReferenceDictionaryJsonConverter))]
        public WeightedVector<StarGenerator> StarGeneratorSelector { get; set; } = new();
        public float TransitLimit { get; set; }
        public float StellarBodyDensity { get; set; }
        public Interval ViableThermalRange { get; set; }
        public Interval ViableGravitationalRange { get; set; }
        public OrbitGenerator? OrbitGenerator { get; set; }
        public StellarBodyGeneratorSelector? StellarBodySelector { get; set; }

        public StarSystem Generate(Vector3 position, GeneratorContext context)
        {
            var random = context.Random;
            var star = StarGeneratorSelector.Get(random.NextSingle()).Generate(context);
            float innerBoundary = 
                Math.Max(
                    GetDistanceForTemperature(star, ViableThermalRange.Maximum),
                    GetDistanceForGravity(star, ViableGravitationalRange.Maximum));
            float outerBoundary =
                Math.Min(
                    GetDistanceForTemperature(star, ViableThermalRange.Minimum), 
                    GetDistanceForGravity(star, ViableGravitationalRange.Minimum));
            float transitLimit = GetDistanceForGravity(star, TransitLimit);
            if (innerBoundary > outerBoundary)
            {
                return new StarSystem(
                    position, 
                    star,
                    new(float.NaN, outerBoundary),
                    new(float.NaN, float.NaN), 
                    transitLimit, 
                    Enumerable.Empty<StellarBody>());
            }

            float numBodiesMean = StellarBodyDensity * (outerBoundary - innerBoundary);
            int numBodies = (int)Math.Round(new NormalSampler(numBodiesMean, 0.5f * numBodiesMean).Generate(random));
            var distanceSampler =
                new ReciprocalSampler(0.01f * (outerBoundary - innerBoundary), 100f, innerBoundary);
            var orbiters = new List<StellarBody>();
            for (int i=0; i<numBodies;++i)
            {
                var orbit = OrbitGenerator!.Generate(star, distanceSampler.Generate(random), context);
                var avgDistance = orbit.GetAverageDistance();
                var generator = StellarBodySelector!.Select(
                        random,
                        GetTemperatureForDistance(star, avgDistance),
                        GetGravityForDistance(star, avgDistance));
                var stellarBody = generator.Generate(orbit, context);
                orbiters.Add(stellarBody);
            }
            orbiters.Sort((x, y) => x.Orbit.GetAverageDistance().CompareTo(y.Orbit.GetAverageDistance()));
            return new StarSystem(
                position, 
                star, 
                new(innerBoundary, outerBoundary), 
                Interval.Intersection(
                    new(innerBoundary, outerBoundary), 
                    new(
                        GetDistanceForTemperature(star, s_GoldilocksThermalRange.Maximum),
                        GetDistanceForTemperature(star, s_GoldilocksThermalRange.Minimum))),
                transitLimit,
                orbiters);
        }

        private static float GetDistanceForTemperature(Star star, float temperature)
        {
            return Constants.AstralUnit * (9 * star.TemperatureK * star.TemperatureK * star.RadiusS) 
                / (16 * temperature * temperature);
        }

        private static float GetTemperatureForDistance(Star star, float distance)
        {
            return 3 * star.TemperatureK * MathF.Sqrt(star.RadiusS)
                / (4 * MathF.Sqrt(distance / Constants.AstralUnit));
        }

        private static float GetDistanceForGravity(Star star, float gravity)
        {
            return Constants.AstralUnit * (float)Math.Sqrt(Constants.GravitationalConstant * star.MassS / gravity);
        }

        private static float GetGravityForDistance(Star star, float distance)
        {
            return Constants.AstralUnit * Constants.AstralUnit * Constants.GravitationalConstant * star.MassS
                / (distance * distance);
        }
    }
}