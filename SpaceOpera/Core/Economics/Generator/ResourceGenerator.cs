using SpaceOpera.Core.Universe;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics.Generator
{
    class ResourceGenerator
    {
        private readonly Sampler _PopulationSampler;
        private readonly float _GasNodeDensity;
        private readonly List<ResourceSampler> _ResourceSamplers;
        private readonly Dictionary<Biome, BiomeResourceGenerator> _ResourceGeneratorsByBiome =
            new Dictionary<Biome, BiomeResourceGenerator>();

        public ResourceGenerator(
            Sampler PopulationSampler,
            float GasNodeDensity, 
            IEnumerable<ResourceSampler> ResourceSamplers)
        {
            _PopulationSampler = PopulationSampler;
            _GasNodeDensity = GasNodeDensity;
            _ResourceSamplers = ResourceSamplers.ToList();
        }

        public void Generate(World World, Random Random)
        {
            foreach (var system in World.Galaxy.Systems)
            {
                foreach (var stellarBody in system.Orbiters)
                {
                    Generate(stellarBody, Random);
                }
            }
        }

        private void Generate(StellarBody StellarBody, Random Random)
        {
            foreach (var region in StellarBody.Regions)
            {
                var resourceNodes = new List<ResourceNode>();
                resourceNodes.AddRange(
                    Generate(region.DominantBiome, Random)
                    .Select(x => new ResourceNode(x.Resource, region.SubRegions.Count * x.Size)));
                foreach (var atmosphereNode in StellarBody.Atmosphere.GetQuantities())
                {
                    resourceNodes.Add(
                        new ResourceNode(
                            atmosphereNode.Value, 
                            (int)Math.Min(
                                region.StructureNodes,
                                _GasNodeDensity * region.StructureNodes * atmosphereNode.Amount)));
                }
                region.AddResources(resourceNodes);
                if (region.Sovereign != null)
                {
                    region.AddPopulation(
                        (uint)(GetPopulationMultiplier(region.DominantBiome) * _PopulationSampler.Sample(Random)));
                }
            }
        }

        private IEnumerable<ResourceNode> Generate(Biome Biome, Random Random)
        {
            return GetOrCreateGenerator(Biome).Generate(Random);
        }

        private double GetPopulationMultiplier(Biome Biome)
        {
            return GetOrCreateGenerator(Biome).PopulationMultiplier;
        }

        private BiomeResourceGenerator GetOrCreateGenerator(Biome Biome)
        {
            _ResourceGeneratorsByBiome.TryGetValue(Biome, out BiomeResourceGenerator generator);
            if (generator == null)
            {
                generator = new BiomeResourceGenerator(Biome, _ResourceSamplers);
                _ResourceGeneratorsByBiome.Add(Biome, generator);
            }
            return generator;
        }
    }
}