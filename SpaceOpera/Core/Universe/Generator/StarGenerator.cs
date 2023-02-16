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

        public Star Generate(GeneratorContext context)
        {
            return new Star(
                Name,
                TemperatureSampler!.Generate(context.Random), 
                Constants.SolarRadius * RadiusSampler!.Generate(context.Random),
                Constants.SolarMass * MassSampler!.Generate(context.Random));
        }
    }
}