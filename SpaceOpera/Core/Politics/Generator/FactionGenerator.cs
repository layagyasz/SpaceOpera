using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics.Generator
{
    class FactionGenerator
    {
        public EnumMap<FactionAttribute, float> BaseAttributes { get; set; }
        public ComponentNameGeneratorGenerator ComponentName { get; set; }

        public Faction Generate(Culture Culture, Banner Banner, Random Random)
        {
            var nameGenerator = new NameGenerator(Culture.Language, ComponentName.Generate(Random));
            return new Faction(nameGenerator.GenerateNameForFaction(Random), Banner, BaseAttributes, nameGenerator);
        }
    }
}
