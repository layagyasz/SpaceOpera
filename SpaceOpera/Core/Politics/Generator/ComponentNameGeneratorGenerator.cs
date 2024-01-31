using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Politics.Generator
{
    public class ComponentNameGeneratorGenerator
    {
        public class ComponentNamePartGenerator
        {
            public float Bits { get; set; }
            public RandomValue? RandomValue { get; set; }
            public ComponentTypeNameGenerator.ComponentNameSource Source { get; set; }
            public ComponentTypeNameGenerator.ComponentNameFilter Filter { get; set; }

            public ComponentTypeNameGenerator.ComponentNamePart Generate(Random random)
            {
                if (Source == ComponentTypeNameGenerator.ComponentNameSource.Static)
                {
                    return new ComponentTypeNameGenerator.ComponentNamePart(
                        RandomValue!.Generate(random), Bits, null, Source, Filter);
                }
                if (Source == ComponentTypeNameGenerator.ComponentNameSource.Random)
                {
                    return new ComponentTypeNameGenerator.ComponentNamePart(null, Bits, RandomValue, Source, Filter);
                }
                return new ComponentTypeNameGenerator.ComponentNamePart(null, Bits, null, Source, Filter);
            }
        }

        public class ComponentNamePattern
        {
            public EnumSet<NameType> SupportedTypes { get; set; } = new();
            public List<string> Pattern { get; set; } = new();
        }

        public EnumSet<NameType> RequiredTypes { get; set; } = new();
        public Dictionary<string, List<Frequent<ComponentNamePartGenerator>>> NameParts { get; set; } = new();
        public List<Frequent<ComponentNamePattern>> Patterns { get; set; } = new();

        public ComponentNameGenerator Generate(GeneratorContext context)
        {
            var random = context.Random;
            var parts = new Dictionary<string, ComponentTypeNameGenerator.ComponentNamePart>();
            foreach (var partGenerator in NameParts)
            {
                if (partGenerator.Value.Count == 0)
                {
                    continue;
                }
                if (partGenerator.Value.Count == 1)
                {
                    parts.Add(partGenerator.Key, partGenerator.Value.First().Value!.Generate(random));
                    continue;
                }
                var options = new WeightedVector<ComponentNamePartGenerator>();
                foreach (var option in partGenerator.Value)
                {
                    options.Add(option.Value!, option.Frequency);
                }
                parts.Add(partGenerator.Key, options.Get(random.NextSingle()).Generate(random));
            }

            var generators = new EnumMap<NameType, ComponentTypeNameGenerator>();
            var requiredTypes = new EnumSet<NameType>(RequiredTypes);
            while (requiredTypes.Count > 0)
            {
                var options = new WeightedVector<ComponentNamePattern>();
                foreach (var pattern in Patterns)
                {
                    if (requiredTypes.Overlaps(pattern.Value!.SupportedTypes))
                    {
                        options.Add(pattern.Value, pattern.Frequency);
                    }
                }
                var selected = options.Get(random.NextSingle());
                var generator = new ComponentTypeNameGenerator(selected.Pattern.Select(x => parts[x]));
                foreach (var type in requiredTypes.Intersect(selected.SupportedTypes))
                {
                    requiredTypes.Remove(type);
                    generators.Add(type, generator);
                }
            }

            return new ComponentNameGenerator(
                generators,
                new List<ComponentTagName>()
                {
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Light), "light"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Medium), ""),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Heavy), "heavy"),

                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Battleship), "battleship"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Carrier), "carrier"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Cruiser), "cruiser"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Destroyer), "destroyer"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Escort), "escort"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Freighter), "freighter"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Frigate), "frigate"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Patrol), "patrol"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Transport), "transport"),

                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Mechanized), "mechanized"),

                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.Infantry), "infantry"),
                    new ComponentTagName(
                        new EnumSet<ComponentTag>(
                            ComponentTag.Unit, ComponentTag.Tank, ComponentTag.Legged),
                        "walker"),
                    new ComponentTagName(
                        new EnumSet<ComponentTag>(ComponentTag.Unit, ComponentTag.Tank, ComponentTag.Tracked), "tank"),
                    new ComponentTagName(
                        new EnumSet<ComponentTag>(
                            ComponentTag.Unit, ComponentTag.Tank, ComponentTag.Wheeled), 
                        "armored car"),

                    new ComponentTagName(
                        new EnumSet<ComponentTag>(ComponentTag.Formation, ComponentTag.Tank), "armored"),
                });
        }
    }
}