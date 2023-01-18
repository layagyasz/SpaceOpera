using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Designs
{
    public class Design
    {
        public DesignConfiguration Configuration { get; }
        public List<ComponentTag> Tags { get; }
        public List<DesignedComponent> Components { get; }
        public List<Recipe> Recipes { get; }

        public Design(
            DesignConfiguration configuration, 
            IEnumerable<ComponentTag> tags,
            IEnumerable<DesignedComponent> components,
            IEnumerable<Recipe> recipes)
        {
            Configuration = configuration;
            Tags = tags.ToList();
            Components = components.ToList();
            Recipes = recipes.ToList();
        }
    }
}