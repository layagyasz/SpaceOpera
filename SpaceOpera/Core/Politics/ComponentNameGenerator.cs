using Cardamom.Collections;
using SpaceOpera.Core.Languages;

namespace SpaceOpera.Core.Politics
{
    public class ComponentNameGenerator
    { 
        private readonly EnumMap<NameType, ComponentTypeNameGenerator> _nameGenerators;
        private readonly List<ComponentTagName> _tagNames;

        public ComponentNameGenerator(
            EnumMap<NameType, ComponentTypeNameGenerator> nameGenerators, IEnumerable<ComponentTagName> tagNames)
        {
            _nameGenerators = nameGenerators;
            _tagNames = tagNames.ToList();
        }

        public string GenerateNameFor(NameGeneratorArgs args, Language language, Random random)
        {
            return _nameGenerators[args.Type].GenerateNameFor(args, language, _tagNames, random);
        }
    }
}