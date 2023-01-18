using Cardamom.Trackers;
using Cardamom.Utils.Generators.Samplers;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Economics.Generator
{
    public class EconomyGenerator
    {
        public ISampler? PopulationSampler { get; set; }
        public float GasNodeDensity { get; set; }
        public float GasNodeEfficiency { get; set; }
        public List<ResourceSampler> ResourceSamplers { get; set; } = new();

        public void Generate(World world, Random random)
        {
            var resources = 
                new ResourceGenerator(PopulationSampler!, GasNodeDensity, ResourceSamplers);
            resources.Generate(world, random);

            foreach (var region in world.Galaxy.Systems.SelectMany(x => x.Orbiters).SelectMany(x => x.Regions))
            {
                if (region.Sovereign != null)
                {
                    world.Economy.CreateSovereignHolding(region.Sovereign, region);
                }
            }

            var totalSinks = new MultiQuantity<IMaterial>();
            foreach (var region in world.Galaxy.Systems.SelectMany(x => x.Orbiters).SelectMany(x => x.Regions))
            {
                if (region.Sovereign != null)
                {
                    foreach (var sink in world.Economy.MaterialSink.PopulationSink)
                    {
                        totalSinks.Add(region.Population * sink.Materials);
                    }
                }
            }

            var recipes = new MultiQuantity<Recipe>();
            foreach (var material in totalSinks.GetQuantities())
            {
                var materialRecipes = material.Value * world.EconomyGraph.GetRequiredRecipes(material.Key);
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
                    recipes.Add(buildMaterial.Amount * world.EconomyGraph.GetRequiredRecipes(buildMaterial.Value));
                }
            }

            foreach (var recipe in recipes.GetQuantities())
            {
                var output = recipe.Key.Transformation.First(x => x.Value > 0);
                var sink =
                    world.Economy.MaterialSink.PopulationSink.FirstOrDefault(
                        x => x.Materials.ContainsKey(output.Key));
                // Recipe produces a sink material.
                if (sink != null)
                {
                    PopulateSinkRecipe(recipe.Value, output.Key, recipe.Value, sink, world);
                }
                // Distribute remaining production randomly among top regions.
            }
        }

        private float PopulateSinkRecipe(Recipe recipe, IMaterial output, SingleMaterialSink sink, World world)
        {
            float distributed = 0;
            float sinkAmount = sink.Materials[output];
            foreach (var stellarBody in world.Galaxy.Systems.SelectMany(x => x.Orbiters))
            {
                var totalNeeded =
                    sinkAmount * stellarBody.Regions.Sum(x => x.Population)
                    / (recipe.Coefficient * recipe.Transformation[output] * recipe.Structure!.MaxWorkers);
                var totalAvailable = 
                    stellarBody.Regions
                        .Where(x => x.Sovereign != null)
                        .Sum(x => GetAvailableResourceNodes(x, recipe.BoundResourceNode));
                var ratio = Math.Min(1, totalNeeded / totalAvailable);

                foreach (var region in stellarBody.Regions)
                {
                    if (region.Sovereign != null)
                    {
                        var holding = world.Economy.GetHolding(region.Sovereign, region);
                        var nodes = ratio * GetAvailableResourceNodes(region, recipe.BoundResourceNode);
                        int numStructures = (int)(distributed + nodes) - (int)(distributed);
                        if (nodes > 0)
                        {
                            holding.AddStructures(new Count<Structure>(recipe.Structure, numStructures));
                            holding.AdjustProduction(new MultiCount<Recipe> { { recipe, numStructures } });
                            distributed += nodes * recipe.Structure.MaxWorkers;
                        }
                    }
                }
            }

            return distributed;
        }

        private long GetAvailableResourceNodes(StellarBodyRegion region, IMaterial? resource)
        {
            int resourceNodes = 
                resource == null ? int.MaxValue : region.Resources.Sum(x => x.Resource == resource ? x.Size : 0);
            return Math.Min(resourceNodes, region.StructureNodes);
        }
    }
}