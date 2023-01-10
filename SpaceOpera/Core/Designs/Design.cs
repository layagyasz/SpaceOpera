using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class Design
    {
        public DesignConfiguration Configuration { get; }
        public List<ComponentTag> Tags { get; }
        public List<DesignedComponent> Components { get; }
        public List<Recipe> Recipes { get; }

        public Design(
            DesignConfiguration Configuration, 
            IEnumerable<ComponentTag> Tags,
            IEnumerable<DesignedComponent> Components,
            IEnumerable<Recipe> Recipes)
        {
            this.Configuration = Configuration;
            this.Tags = Tags.ToList();
            this.Components = Components.ToList();
            this.Recipes = Recipes.ToList();
        }
    }
}