using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class EconomyGraph
    {
        class RecipeNode
        {
            public Recipe Recipe { get; }
            public MultiQuantity<MaterialNode> Ancestors { get; } 

            public RecipeNode(Recipe Recipe, MultiQuantity<MaterialNode> Ancestors)
            {
                this.Recipe = Recipe;
                this.Ancestors = Ancestors;
            }

            public MultiQuantity<Recipe> Rollup()
            {
                var result = new MultiQuantity<Recipe>();
                foreach (var ancestor in Ancestors.GetQuantities())
                {
                    result.Add(ancestor.Amount * ancestor.Value.Rollup());
                }
                return result;
            }
        }

        class MaterialNode
        {
            public IMaterial Material { get; }
            public MultiQuantity<RecipeNode> Ancestors { get; }

            public MaterialNode(IMaterial Material, MultiQuantity<RecipeNode> Ancestors)
            {
                this.Material = Material;
                this.Ancestors = Ancestors;
            }

            public MultiQuantity<Recipe> Rollup()
            {
                var result = new MultiQuantity<Recipe>();
                foreach (var ancestor in Ancestors.GetQuantities())
                {
                    result.Add(ancestor.Value.Recipe, ancestor.Amount);
                    result.Add(ancestor.Amount * ancestor.Value.Rollup());
                }
                return result;
            }
        }

        private readonly List<Recipe> _Recipes = new List<Recipe>();

        private readonly Dictionary<Recipe, RecipeNode> _RecipeNodes = new Dictionary<Recipe, RecipeNode>();
        private readonly Dictionary<IMaterial, MaterialNode> _MaterialNodes =
            new Dictionary<IMaterial, MaterialNode>();

        public void AddRecipe(Recipe Recipe)
        {
            _Recipes.Add(Recipe);
        }

        public void AddRecipes(IEnumerable<Recipe> Recipes)
        {
            _Recipes.AddRange(Recipes);
        }

        public MultiQuantity<Recipe> GetRequiredRecipes(IMaterial Material)
        {
            return GetOrCreateMaterialNode(Material).Rollup();
        }

        private MaterialNode CreateMaterialNode(IMaterial Material)
        {
            var ancestors = new MultiQuantity<RecipeNode>();
            foreach (var recipe in _Recipes.Where(x => x.Transformation.Any(y => y.Key == Material && y.Value > 0)))
            {
                ancestors.Add(
                    GetOrCreateRecipeNode(recipe), 
                    1 / (recipe.Coefficient * recipe.Transformation.First(x => x.Key == Material).Value));
            }
            return new MaterialNode(Material, ancestors);
        }

        private RecipeNode CreateRecipeNode(Recipe Recipe)
        {
            var ancestors = new MultiQuantity<MaterialNode>();
            foreach (var material in Recipe.Transformation.Where(x => x.Value < 0))
            {
                ancestors.Add(GetOrCreateMaterialNode(material.Key), -Recipe.Coefficient * material.Value);
            }
            return new RecipeNode(Recipe, ancestors);
        }

        private MaterialNode GetOrCreateMaterialNode(IMaterial Material)
        {
            _MaterialNodes.TryGetValue(Material, out MaterialNode value);
            if (value == null)
            {
                value = CreateMaterialNode(Material);
                _MaterialNodes.Add(Material, value);
            }
            return value;
        }

        private RecipeNode GetOrCreateRecipeNode(Recipe Recipe)
        {
            _RecipeNodes.TryGetValue(Recipe, out RecipeNode value);
            if (value == null)
            {
                value = CreateRecipeNode(Recipe);
                _RecipeNodes.Add(Recipe, value);
            }
            return value;
        }
    }
}