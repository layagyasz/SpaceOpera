using SpaceOpera.Core.Economics;
using System.ComponentModel;
using System.Drawing;

namespace SpaceOpera.Core.Designs
{
    public class Design
    {
        public string Name => Configuration.Name;
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

        public void SetName(string name)
        {
            Configuration.SetName(name);
            foreach (var component in Components)
            {
                component.Name = 
                    string.Format("{0} ({1})", name, StringUtils.FormatEnumChar(component.Slot.Size.ToString()));
            }
            foreach (var recipe in Recipes)
            {
                recipe.Name = name;
            }
        }
    }
}