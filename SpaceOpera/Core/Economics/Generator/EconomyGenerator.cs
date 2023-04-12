using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Trackers;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics.Generator
{
    public class EconomyGenerator
    {
        public ResourceGenerator? Resources { get; set; }

        public void Generate(World world, GeneratorContext context)
        {
            Resources!.Generate(world, context);
            var holdings = new MultiMap<Faction, StellarBodyRegionHolding>();
            foreach (var region in world.Galaxy.Systems.SelectMany(x => x.Orbiters).SelectMany(x => x.Regions))
            {
                if (region.Sovereign != null)
                {
                    holdings.Add(region.Sovereign, world.Economy.CreateSovereignHolding(region.Sovereign, region));
                }
            }
            foreach (var faction in world.GetFactions())
            {
                Generate(world, holdings[faction], context);
            }
        }

        private static void Generate(
            World world, IEnumerable<StellarBodyRegionHolding> holdings, GeneratorContext context)
        {
            var totalSinks = new MultiQuantity<IMaterial>();
            foreach (var holding in holdings)
            {
                foreach (var sink in world.Economy.MaterialSink.PopulationSink)
                {
                    totalSinks.Add(holding.Region.Population * sink.Materials);
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
                            x => x.Value / (4 * x.Key.Structure!.BuildTime * x.Key.Structure!.MaxWorkers)
                                * x.Key.Structure!.Cost)
                        .GroupBy(x => x.Key)
                        .ToMultiQuantity(x => x.Key, x => x.Sum(y => y.Value));
                foreach (var buildMaterial in buildMaterials.GetQuantities())
                {
                    recipes.Add(buildMaterial.Value * world.EconomyGraph.GetRequiredRecipes(buildMaterial.Key));
                }
            }

            foreach (var recipe in recipes.GetQuantities())
            {
                PopulateRecipe(recipe.Key, recipe.Value, holdings);
                // Trade for remaining demand.
            }
        }

        private static float PopulateRecipe(
            Recipe recipe, float recipeAmount, IEnumerable<StellarBodyRegionHolding> holdings)
        {
            int neededStructures = (int)Math.Ceiling(recipeAmount / recipe.Structure!.MaxWorkers);
            float distributed = 0;
            foreach (var holding in holdings)
            {
                var nodes = holding.GetAvailableResourceNodes(recipe.BoundResourceNode);
                int numStructures = Math.Min(neededStructures, Math.Min(holding.GetAvailableStructureNodes(), nodes));
                if (numStructures > 0)
                {
                    holding.AddStructures(Count<Structure>.Create(recipe.Structure, numStructures));
                    holding.AdjustProduction(new MultiCount<Recipe> { { recipe, numStructures } });
                    neededStructures -= numStructures;
                    distributed += nodes * recipe.Structure.MaxWorkers;
                }
                if (neededStructures <= 0)
                {
                    return 0;
                }
            }
            return recipeAmount - distributed;
        }
    }
}