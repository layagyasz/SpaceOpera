using Cardamom.Collections;
using Cardamom.Mathematics;
using Cardamom.Utils.Generators.Samplers;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StarSystemGenerator
    {
        public WeightedVector<StarGenerator> StarGenerator { get; set; } = new();
        public float TransitLimit { get; set; }
        public float StellarBodyDensity { get; set; }
        public Interval ViableThermalRange { get; set; }
        public Interval ViableGravitationalRange { get; set; }
        public OrbitGenerator? OrbitGenerator { get; set; }
        public StellarBodyGeneratorSelector? StellarBodySelector { get; set; }

        public StarSystem Generate(Random random, Vector2 position)
        {
            var star = StarGenerator.Get(random.NextSingle()).Generate(random);
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
                    position, star, float.NaN, outerBoundary, transitLimit, Enumerable.Empty<StellarBody>());
            }

            float numBodiesMean = StellarBodyDensity * (outerBoundary - innerBoundary);
            int numBodies = (int)Math.Round(new NormalSampler(numBodiesMean, 0.5f * numBodiesMean).Generate(random));
            var distanceSampler = new ReciprocalSampler(innerBoundary, outerBoundary / innerBoundary);
            var orbiters = new List<StellarBody>();
            for (int i=0; i<numBodies;++i)
            {
                var orbit = OrbitGenerator!.Generate(random, star, distanceSampler.Generate(random));
                var avgDistance = orbit.GetAverageDistance();
                var generator = StellarBodySelector!.Select(
                        random,
                        GetTemperatureForDistance(star, avgDistance),
                        GetGravityForDistance(star, avgDistance));
                var stellarBody = generator.Generate(random, orbit);
                orbiters.Add(stellarBody);
            }
            orbiters.Sort((x, y) => x.Orbit.GetAverageDistance().CompareTo(y.Orbit.GetAverageDistance()));
            return new StarSystem(position, star, innerBoundary, outerBoundary, transitLimit, orbiters);
        }

        private static float GetDistanceForTemperature(Star star, float temperature)
        {
            return Constants.AstralUnit * (9 * star.Temperature * star.Temperature * star.Radius) 
                / (16 * temperature * temperature);
        }

        private static float GetTemperatureForDistance(Star star, float distance)
        {
            return (float)((3 * star.Temperature * Math.Sqrt(star.Radius)) 
                / (4 * Math.Sqrt(distance / Constants.AstralUnit)));
        }

        private static float GetDistanceForGravity(Star star, float gravity)
        {
            return Constants.AstralUnit * (float)Math.Sqrt(Constants.GravitationalConstant * star.Mass / gravity);
        }

        private static float GetGravityForDistance(Star star, float distance)
        {
            return (Constants.AstralUnit * Constants.AstralUnit * Constants.GravitationalConstant * star.Mass)
                / (distance * distance);
        }
    }
}