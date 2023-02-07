using Cardamom.Utils.Generators.Samplers;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics.Generator
{
    public class ResourceGenerator
    {
        private readonly ISampler _populationSampler;
        private readonly float _gasNodeDensity;
        private readonly List<ResourceSampler> _resourceSamplers;
        private readonly Dictionary<Biome, BiomeResourceGenerator> _resourceGeneratorsByBiome = new();

        public ResourceGenerator(
            ISampler populationSampler,
            float gasNodeDensity, 
            IEnumerable<ResourceSampler> resourceSamplers)
        {
            _populationSampler = populationSampler;
            _gasNodeDensity = gasNodeDensity;
            _resourceSamplers = resourceSamplers.ToList();
        }

        public void Generate(World world, Random random)
        {
            foreach (var system in world.Galaxy.Systems)
            {
                foreach (var stellarBody in system.Orbiters)
                {
                    Generate(stellarBody, random);
                }
            }
        }

        private void Generate(StellarBody stellarBody, Random random)
        {
            foreach (var region in stellarBody.Regions)
            {
                var resourceNodes = new List<ResourceNode>();
                resourceNodes.AddRange(
                    Generate(region.DominantBiome, random)
                    .Select(x => new ResourceNode(x.Resource, region.SubRegions.Count * x.Size)));
                foreach (var atmosphereNode in stellarBody.Atmosphere.Composition.GetQuantities())
                {
                    resourceNodes.Add(
                        new ResourceNode(
                            atmosphereNode.Key, 
                            (int)Math.Min(
                                region.StructureNodes,
                                _gasNodeDensity * region.StructureNodes * atmosphereNode.Value)));
                }
                region.AddResources(resourceNodes);
                if (region.Sovereign != null)
                {
                    region.AddPopulation(
                        (uint)(GetPopulationMultiplier(region.DominantBiome) * _populationSampler.Generate(random)));
                }
            }
        }

        private IEnumerable<ResourceNode> Generate(Biome biome, Random random)
        {
            return GetOrCreateGenerator(biome).Generate(random);
        }

        private double GetPopulationMultiplier(Biome biome)
        {
            return GetOrCreateGenerator(biome).PopulationMultiplier;
        }

        private BiomeResourceGenerator GetOrCreateGenerator(Biome biome)
        {
            _resourceGeneratorsByBiome.TryGetValue(biome, out var generator);
            if (generator == null)
            {
                generator = new BiomeResourceGenerator(biome, _resourceSamplers);
                _resourceGeneratorsByBiome.Add(biome, generator);
            }
            return generator;
        }
    }
}