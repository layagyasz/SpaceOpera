using Cardamom.Collections;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StellarBodyGeneratorSelector
    {
        public List<StellarBodyGeneratorOption> Options { get; set; } = new();

        public StellarBodyGenerator Select(Random random, float temperature, float gravity)
        {
            var weights = new WeightedVector<StellarBodyGeneratorOption>();
            foreach (var option in Options)
            {
                if (option.Satisfies(temperature, gravity))
                {
                    weights.Add(option, option.Weight);
                }
            }
            return weights.Get(random.NextSingle()).Generator!;
        }
    }
}
