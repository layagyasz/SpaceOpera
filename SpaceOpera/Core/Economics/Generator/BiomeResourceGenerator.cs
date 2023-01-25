using Cardamom.Trackers;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics.Generator
{
    public class BiomeResourceGenerator
    {
        public Biome Biome { get; }
        public float PopulationMultiplier { get; }

        private readonly MultiQuantity<ResourceSampler> _resourceSamplers;

        public BiomeResourceGenerator(Biome biome, IEnumerable<ResourceSampler> resourceSamplers)
        {
            Biome = biome;
            PopulationMultiplier = 
                (Modifier.One + GameModifier.AggregatePopulationGeneration(biome.Modifiers)).GetTotal();

            var modifiers = GameModifier.AggregateResourceGeneration(biome.Modifiers);
            _resourceSamplers = new MultiQuantity<ResourceSampler>();
            foreach (var sampler in resourceSamplers)
            {
                modifiers.TryGetValue(sampler.Resource!, out var modifier);
                var total = (Modifier.One + modifier).GetTotal();
                if (total > 0)
                {
                    _resourceSamplers.Add(sampler, total);
                }
            }
        }

        public IEnumerable<ResourceNode> Generate(Random random)
        {
            return _resourceSamplers.Where(x => x.Key.Appears(random)).Select(x => x.Key.Generate(x.Value, random));
        }
    }
}