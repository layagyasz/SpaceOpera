using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceOpera.Core.Politics.ComponentNameGenerator;

namespace SpaceOpera.Core.Politics.Generator
{
    class ComponentNameGeneratorGenerator
    {
        public class ComponentNamePartGenerator
        {
            public RandomValue RandomValue { get; set; }
            public ComponentTypeNameGenerator.ComponentNameSource Source { get; set; }
            public ComponentTypeNameGenerator.ComponentNameFilter Filter { get; set; }

            public ComponentTypeNameGenerator.ComponentNamePart Generate(Random Random)
            {
                if (Source == ComponentTypeNameGenerator.ComponentNameSource.STATIC)
                {
                    return new ComponentTypeNameGenerator.ComponentNamePart(
                        RandomValue.Generate(Random), null, Source, Filter);
                }
                if (Source == ComponentTypeNameGenerator.ComponentNameSource.RANDOM)
                {
                    return new ComponentTypeNameGenerator.ComponentNamePart(null, RandomValue, Source, Filter);
                }
                return new ComponentTypeNameGenerator.ComponentNamePart(null, null, Source, Filter);
            }
        }

        public class ComponentNamePattern
        {
            public EnumSet<NameType> SupportedTypes { get; set; }
            public List<string> Pattern { get; set; }
        }

        public EnumSet<NameType> RequiredTypes { get; set; }
        public Dictionary<string, List<Frequent<ComponentNamePartGenerator>>> NameParts { get; set; }
        public List<Frequent<ComponentNamePattern>> Patterns { get; set; }

        public ComponentNameGenerator Generate(Random Random)
        {
            var parts = new Dictionary<string, ComponentTypeNameGenerator.ComponentNamePart>();
            foreach (var partGenerator in NameParts)
            {
                if (partGenerator.Value.Count == 0)
                {
                    continue;
                }
                if (partGenerator.Value.Count == 1)
                {
                    parts.Add(partGenerator.Key, partGenerator.Value.First().Value.Generate(Random));
                    continue;
                }
                var options = new WeightedVector<ComponentNamePartGenerator>();
                foreach (var option in partGenerator.Value)
                {
                    options.Add(option.Frequency, option.Value);
                }
                parts.Add(partGenerator.Key, options[Random.NextDouble()].Generate(Random));
            }

            var generators = new EnumMap<NameType, ComponentTypeNameGenerator>();
            var requiredTypes = new EnumSet<NameType>(RequiredTypes);
            while (requiredTypes.Count > 0)
            {
                var options = new WeightedVector<ComponentNamePattern>();
                foreach (var pattern in Patterns)
                {
                    if (requiredTypes.Overlaps(pattern.Value.SupportedTypes))
                    {
                        options.Add(pattern.Frequency, pattern.Value);
                    }
                }
                var selected = options[Random.NextDouble()];
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
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.LIGHT), "light"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.MEDIUM), ""),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.HEAVY), "heavy"),

                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.BATTLESHIP), "battleship"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.CARRIER), "carrier"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.CRUISER), "cruiser"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.DESTROYER), "destroyer"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.ESCORT), "escort"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.FREIGHTER), "freighter"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.FRIGATE), "frigate"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.PATROL), "patrol"),
                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.TRANSPORT), "transport"),

                    new ComponentTagName(new EnumSet<ComponentTag>(ComponentTag.INFANTRY), "infantry")
                });
        }
    }
}