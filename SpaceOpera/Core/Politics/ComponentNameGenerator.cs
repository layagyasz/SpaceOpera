using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class ComponentNameGenerator
    { 
        private readonly EnumMap<NameType, ComponentTypeNameGenerator> _NameGenerators;
        private readonly List<ComponentTagName> _TagNames;

        public ComponentNameGenerator(
            EnumMap<NameType, ComponentTypeNameGenerator> NameGenerators, IEnumerable<ComponentTagName> TagNames)
        {
            _NameGenerators = NameGenerators;
            _TagNames = TagNames.ToList();
        }

        public string GenerateNameFor(NameGeneratorArgs Args, Language Language, Random Random)
        {
            return _NameGenerators[Args.Type].GenerateNameFor(Args, Language, _TagNames, Random);
        }
    }
}