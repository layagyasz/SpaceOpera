using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics.Generator
{
    class BiomeResourceGenerator
    {
        public Biome Biome { get; }
        public float PopulationMultiplier { get; }

        private readonly MultiQuantity<ResourceSampler> _ResourceSamplers;

        public BiomeResourceGenerator(Biome Biome, IEnumerable<ResourceSampler> ResourceSamplers)
        {
            this.Biome = Biome;
            this.PopulationMultiplier = 
                (Modifier.ONE + GameModifier.AggregatePopulationGeneration(Biome.Modifiers)).GetTotal();

            var modifiers = GameModifier.AggregateResourceGeneration(Biome.Modifiers);
            _ResourceSamplers = new MultiQuantity<ResourceSampler>();
            foreach (var sampler in ResourceSamplers)
            {
                modifiers.TryGetValue(sampler.Resource, out Modifier modifier);
                var total = (Modifier.ONE + modifier).GetTotal();
                if (total > 0)
                {
                    _ResourceSamplers.Add(sampler, total);
                }
            }
        }

        public IEnumerable<ResourceNode> Generate(Random Random)
        {
            return _ResourceSamplers.Where(x => x.Key.Appears(Random)).Select(x => x.Key.Generate(x.Value, Random));
        }
    }
}