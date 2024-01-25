using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core.Designs
{
    public class Design
    {
        public string Name => Configuration.Name;
        public DesignConfiguration Configuration { get; }
        public ComponentTag[] Tags { get; }
        public DesignedComponent[] Components { get; }
        public Recipe[] Recipes { get; }

        public Design(
            DesignConfiguration configuration, 
            IEnumerable<ComponentTag> tags,
            IEnumerable<DesignedComponent> components,
            IEnumerable<Recipe> recipes)
        {
            Configuration = configuration;
            Tags = tags.ToArray();
            Components = components.ToArray();
            Recipes = recipes.ToArray();
        }

        public void SetName(string name)
        {
            Configuration.SetName(name);
            foreach (var component in Components)
            {
                if (Configuration.Template.Sizes.Count == 0) 
                {
                    component.Name = name;
                } 
                else
                {
                    component.Name = $"{name} ({ToSizeString(component.Slot.Size)})";
                }
            }
            foreach (var recipe in Recipes)
            {
                recipe.Name = name;
            }
        }

        public static string ToSizeString(ComponentSize size)
        {
            switch (size)
            {
                case ComponentSize.Tiny:
                    return "T";
                case ComponentSize.ExtraSmall:
                    return "XS";
                case ComponentSize.PointDefense:
                    return "PD";
                case ComponentSize.Small:
                    return "S";
                case ComponentSize.Medium:
                    return "M";
                case ComponentSize.Large:
                    return "L";
                default:
                    throw new ArgumentException($"Unsupported ComponentSize: [{size}]");
            }
        }
    }
}