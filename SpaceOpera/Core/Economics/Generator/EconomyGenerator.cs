using Cardamom.Utilities;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics.Generator
{
    class EconomyGenerator
    {
        public Sampler PopulationSampler { get; set; }
        public float GasNodeDensity { get; set; }
        public float GasNodeEfficiency { get; set; }
        public List<ResourceSampler> ResourceSamplers { get; set; }

        public void Generate(World World, Random Random)
        {
            var resources = 
                new ResourceGenerator(PopulationSampler, GasNodeDensity, ResourceSamplers);
            resources.Generate(World, Random);

            foreach (var region in World.Galaxy.Systems.SelectMany(x => x.Orbiters).SelectMany(x => x.Regions))
            {
                if (region.Sovereign != null)
                {
                    World.Economy.CreateSovereignHolding(region.Sovereign, region);
                }
            }

            var totalSinks = new MultiQuantity<IMaterial>();
            foreach (var region in World.Galaxy.Systems.SelectMany(x => x.Orbiters).SelectMany(x => x.Regions))
            {
                if (region.Sovereign != null)
                {
                    foreach (var sink in World.Economy.MaterialSink.PopulationSink)
                    {
                        totalSinks.Add(region.Population * sink.Materials);
                    }
                }
            }

            var recipes = new MultiQuantity<Recipe>();
            foreach (var material in totalSinks.GetQuantities())
            {
                var materialRecipes = material.Amount * World.EconomyGraph.GetRequiredRecipes(material.Value);
                recipes.Add(materialRecipes);
                var buildMaterials =
                    materialRecipes
                        .GetQuantities()
                        .SelectMany(
                        x => x.Amount / (4 * x.Value.Structure.BuildTime * x.Value.Structure.MaxWorkers) 
                            * x.Value.Structure.Cost)
                        .GroupBy(x => x.Key)
                        .ToMultiQuantity(x => x.Key, x => x.Sum(y => y.Value));
                foreach (var buildMaterial in buildMaterials.GetQuantities())
                {
                    recipes.Add(buildMaterial.Amount * World.EconomyGraph.GetRequiredRecipes(buildMaterial.Value));
                }
            }

            foreach (var recipe in recipes.GetQuantities())
            {
                var output = recipe.Value.Transformation.First(x => x.Value > 0);
                var sink =
                    World.Economy.MaterialSink.PopulationSink.FirstOrDefault(
                        x => x.Materials.ContainsKey(output.Key));
                // Recipe produces a sink material.
                if (sink != null)
                {
                    PopulateSinkRecipe(recipe.Value, output.Key, recipe.Amount, sink, World);
                }
                // Distribute remaining production randomly among top regions.
            }
        }

        private float PopulateSinkRecipe(
            Recipe Recipe, IMaterial Output, float RecipeAmount, SingleMaterialSink Sink, World World)
        {
            float distributed = 0;
            float sinkAmount = Sink.Materials[Output];
            foreach (var stellarBody in World.Galaxy.Systems.SelectMany(x => x.Orbiters))
            {
                var totalNeeded =
                    sinkAmount * stellarBody.Regions.Sum(x => x.Population)
                    / (Recipe.Coefficient * Recipe.Transformation[Output] * Recipe.Structure.MaxWorkers);
                var totalAvailable = 
                    stellarBody.Regions
                        .Where(x => x.Sovereign != null)
                        .Sum(x => GetAvailableResourceNodes(x, Recipe.BoundResourceNode));
                var ratio = Math.Min(1, totalNeeded / totalAvailable);

                foreach (var region in stellarBody.Regions)
                {
                    if (region.Sovereign != null)
                    {
                        var holding = World.Economy.GetHolding(region.Sovereign, region);
                        var nodes = ratio * GetAvailableResourceNodes(region, Recipe.BoundResourceNode);
                        int numStructures = (int)(distributed + nodes) - (int)(distributed);
                        if (nodes > 0)
                        {
                            holding.AddStructures(new Count<Structure>(Recipe.Structure, numStructures));
                            holding.AdjustProduction(new MultiCount<Recipe> { { Recipe, numStructures } });
                            distributed += nodes * Recipe.Structure.MaxWorkers;
                        }
                    }
                }
            }

            return distributed;
        }

        private long GetAvailableResourceNodes(StellarBodyRegion Region, IMaterial Resource)
        {
            int resourceNodes = 
                Resource == null ? int.MaxValue : Region.Resources.Sum(x => x.Resource == Resource ? x.Size : 0);
            return Math.Min(resourceNodes, Region.StructureNodes);
        }
    }
}