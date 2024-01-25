using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Designs
{
    public class DesignBuilder
    {
        public ComponentClassifier UnitClassifier { get; }

        public DesignBuilder(ComponentClassifier unitClassifier)
        {
            UnitClassifier = unitClassifier;
        }

        public Design Build(DesignConfiguration design)
        {
            var tags = UnitClassifier.Classify(BuildExemplar(design)).ToArray();
            var components = BuildComponents(design, tags.ToMultiCount(x => x, _ => 1)).ToList();
            var recipes = 
                components
                    .Where(x => x is DesignedMaterial)
                    .Cast<DesignedMaterial>()
                    .Select(x => Recipe.ForDesignedMaterial(x, design.Template.Structure!))
                    .ToList();
            return new Design(design, tags, components, recipes);
        }

        private static DesignedComponent BuildExemplar(DesignConfiguration design)
        {
            return BuildComponent(
                string.Empty,
                new ComponentSlot() { Size = ComponentSize.Unknown, Type = design.Template.Type },
                design.GetComponents(),
                design.GetTags());
        }

        private static IEnumerable<DesignedComponent> BuildComponents(
            DesignConfiguration design, MultiCount<ComponentTag> tags)
        {
            if (design.Template.Sizes.Count == 0)
            {
                yield return BuildComponent(
                    design.Name, 
                    new ComponentSlot() { Size = ComponentSize.Unknown, Type = design.Template.Type },
                    design.GetComponents(), 
                    tags);
            }
            else
            {
                foreach (var size in design.Template.Sizes)
                {
                    var components = design.GetComponents().ToList();
                    components.Add(new(1, size.Value));
                    yield return BuildComponent(
                        $"{design.Name} ({Design.ToSizeString(size.Key)})",
                        new ComponentSlot() { Size = size.Key, Type = design.Template.Type },
                        components,
                        tags);
                }
            }
        }

        private static DesignedComponent BuildComponent(
            string name, ComponentSlot slot, IEnumerable<ComponentAndWeight> components, MultiCount<ComponentTag> tags)
        {
            return slot.Type switch
            {
                ComponentType.Infantry or ComponentType.Ship or ComponentType.Vehicle => 
                    new Unit(name, slot, components, tags),
                ComponentType.BattalionTemplate => new BattalionTemplate(name, slot, components, tags),
                ComponentType.DivisionTemplate => new DivisionTemplate(name, slot, components, tags),
                _ => new DesignedComponent(name, slot, components, tags),
            };
        }
    }
}