using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    class DesignBuilder
    {
        public ComponentClassifier UnitClassifier { get; }

        public DesignBuilder(ComponentClassifier UnitClassifier)
        {
            this.UnitClassifier = UnitClassifier;
        }

        public Design Build(DesignConfiguration Design)
        {
            var tags = UnitClassifier.Classify(Design);
            var components = BuildComponents(Design, tags).ToList();
            var recipes = 
                components
                    .Where(x => x is DesignedMaterial)
                    .Cast<DesignedMaterial>()
                    .Select(x => Recipe.ForDesignedMaterial(x, Design.Template.Structure))
                    .ToList();
            return new Design(Design, tags, components, recipes);
        }

        private IEnumerable<DesignedComponent> BuildComponents(DesignConfiguration Design, IEnumerable<ComponentTag> Tags)
        {
            if (Design.Template.Sizes == null)
            {
                yield return BuildComponent(
                    Design.Name, 
                    new ComponentSlot() { Size = ComponentSize.NONE, Type = Design.Template.Type },
                    Design.GetComponents(), 
                    Tags);
            }
            else
            {
                foreach (var size in Design.Template.Sizes)
                {
                    var components = Design.GetComponents().ToList();
                    components.Add(new ComponentAndSlot(new DesignSlot() { Count = 1, Weight = 1 }, size.Value));
                    yield return BuildComponent(
                        string.Format("{0} ({1})", Design.Name, StringUtils.FormatEnumChar(size.Key.ToString())),
                        new ComponentSlot() { Size = size.Key, Type = Design.Template.Type },
                        components,
                        Tags);
                }
            }
        }

        private DesignedComponent BuildComponent(
            string Name, ComponentSlot Slot, IEnumerable<ComponentAndSlot> Components, IEnumerable<ComponentTag> Tags)
        {
            switch (Slot.Type)
            {
                case ComponentType.INFANTRY:
                case ComponentType.SHIP:
                    return new Unit(Name, Slot, Components, Tags);
                case ComponentType.BATTALION_TEMPLATE:
                    return new BattalionTemplate(Name, Slot, Components, Tags);
                case ComponentType.DIVISION_TEMPLATE:
                    return new DivisionTemplate(Name, Slot, Components, Tags);
                default:
                    return new DesignedComponent(Name, Slot, Components, Tags);
            }
        }
    }
}