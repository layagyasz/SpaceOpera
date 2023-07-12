using Cardamom.Utils.Generators.Samplers;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics.Generator
{
    public class ResourceGenerator
    {
        public ISampler? PopulationSampler { get; set; }
        public float GasNodeDensity { get; set; }
        public List<ResourceSampler> ResourceSamplers { get; set; } = new();

        private readonly Dictionary<Biome, BiomeResourceGenerator> _resourceGeneratorsByBiome = new();

        public void Generate(World world, GeneratorContext context)
        {
            context.LoaderStatus!.AddWork(WorldGenerator.Step.Resources, world.Galaxy.Systems.Count);
            int i = 0;
            foreach (var system in world.Galaxy.Systems)
            {
                context.LoaderStatus!.SetStatus(
                    WorldGenerator.Step.Resources, $"Populating {i + 1}/{world.Galaxy.Systems.Count}");
                foreach (var stellarBody in system.Orbiters)
                {
                    Generate(stellarBody, context);
                }
                context.LoaderStatus!.DoWork(WorldGenerator.Step.Resources);
                ++i;
            }
        }

        private void Generate(StellarBody stellarBody, GeneratorContext context)
        {
            var random = context.Random;
            foreach (var region in stellarBody.Regions)
            {
                var resourceNodes = new List<ResourceNode>();
                foreach (var subRegion in region.SubRegions)
                {
                    resourceNodes.AddRange(
                        Generate(subRegion.Biome, random).Select(x => new ResourceNode(x.Resource, x.Size)));
                }
                foreach (var atmosphereNode in stellarBody.Atmosphere.Composition.GetQuantities())
                {
                    resourceNodes.Add(
                        new ResourceNode(
                            atmosphereNode.Key, 
                            (int)Math.Min(
                                region.StructureNodes,
                                GasNodeDensity * region.StructureNodes * atmosphereNode.Value)));
                }
                region.AddResources(resourceNodes);
                if (region.Sovereign != null)
                {
                    region.AddPopulation(
                        (uint)(GetPopulationMultiplier(region.DominantBiome) * PopulationSampler!.Generate(random)));
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
                generator = new BiomeResourceGenerator(biome, ResourceSamplers);
                _resourceGeneratorsByBiome.Add(biome, generator);
            }
            return generator;
        }
    }
}