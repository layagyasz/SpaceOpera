using Cardamom;
using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StarGenerator : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ISampler? TemperatureSampler { get; set; }
        public ISampler? RadiusSampler { get; set; }
        public ISampler? MassSampler { get; set; }

        public Star Generate(Random random)
        {
            return new Star(
                TemperatureSampler!.Generate(random), 
                Constants.SolarRadius * RadiusSampler!.Generate(random),
                Constants.SolarMass * MassSampler!.Generate(random));
        }
    }
}