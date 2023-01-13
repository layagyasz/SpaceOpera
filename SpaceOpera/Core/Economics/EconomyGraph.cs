using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics
{
    public class EconomyGraph
    {
        class RecipeNode
        {
            public Recipe Recipe { get; }
            public MultiQuantity<MaterialNode> Ancestors { get; } 

            public RecipeNode(Recipe recipe, MultiQuantity<MaterialNode> ancestors)
            {
                Recipe = recipe;
                Ancestors = ancestors;
            }

            public MultiQuantity<Recipe> Rollup()
            {
                var result = new MultiQuantity<Recipe>();
                foreach (var ancestor in Ancestors.GetQuantities())
                {
                    result.Add(ancestor.Value * ancestor.Key.Rollup());
                }
                return result;
            }
        }

        class MaterialNode
        {
            public IMaterial Material { get; }
            public MultiQuantity<RecipeNode> Ancestors { get; }

            public MaterialNode(IMaterial material, MultiQuantity<RecipeNode> ancestors)
            {
                Material = material;
                Ancestors = ancestors;
            }

            public MultiQuantity<Recipe> Rollup()
            {
                var result = new MultiQuantity<Recipe>();
                foreach (var ancestor in Ancestors.GetQuantities())
                {
                    result.Add(ancestor.Key.Recipe, ancestor.Value);
                    result.Add(ancestor.Value * ancestor.Key.Rollup());
                }
                return result;
            }
        }

        private readonly List<Recipe> _recipes = new();

        private readonly Dictionary<Recipe, RecipeNode> _recipeNodes = new();
        private readonly Dictionary<IMaterial, MaterialNode> _materialNodes = new();

        public void AddRecipe(Recipe recipe)
        {
            _recipes.Add(recipe);
        }

        public void AddRecipes(IEnumerable<Recipe> recipes)
        {
            _recipes.AddRange(recipes);
        }

        public MultiQuantity<Recipe> GetRequiredRecipes(IMaterial material)
        {
            return GetOrCreateMaterialNode(material).Rollup();
        }

        private MaterialNode CreateMaterialNode(IMaterial material)
        {
            var ancestors = new MultiQuantity<RecipeNode>();
            foreach (var recipe in _recipes.Where(x => x.Transformation.Any(y => y.Key == material && y.Value > 0)))
            {
                ancestors.Add(
                    GetOrCreateRecipeNode(recipe), 
                    1 / (recipe.Coefficient * recipe.Transformation.First(x => x.Key == material).Value));
            }
            return new MaterialNode(material, ancestors);
        }

        private RecipeNode CreateRecipeNode(Recipe recipe)
        {
            var ancestors = new MultiQuantity<MaterialNode>();
            foreach (var material in recipe.Transformation.Where(x => x.Value < 0))
            {
                ancestors.Add(GetOrCreateMaterialNode(material.Key), -recipe.Coefficient * material.Value);
            }
            return new RecipeNode(recipe, ancestors);
        }

        private MaterialNode GetOrCreateMaterialNode(IMaterial material)
        {
            _materialNodes.TryGetValue(material, out var value);
            if (value == null)
            {
                value = CreateMaterialNode(material);
                _materialNodes.Add(material, value);
            }
            return value;
        }

        private RecipeNode GetOrCreateRecipeNode(Recipe recipe)
        {
            _recipeNodes.TryGetValue(recipe, out var value);
            if (value == null)
            {
                value = CreateRecipeNode(recipe);
                _recipeNodes.Add(recipe, value);
            }
            return value;
        }
    }
}