using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Universe.Generator
{
    public class OrbitGenerator
    {
        public ISampler? EccentricitySampler { get; set; }

        public Orbit Generate(Random random, Star star, float semiMajorAxis)
        {
            return new Orbit(star, 2 * semiMajorAxis, EccentricitySampler!.Generate(random));
        }
    }
}
