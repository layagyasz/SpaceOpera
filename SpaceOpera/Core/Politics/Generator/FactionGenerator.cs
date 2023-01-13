using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Generator
{
    public class FactionGenerator
    {
        public EnumMap<FactionAttribute, float> BaseAttributes { get; set; } = new();
        public ComponentNameGeneratorGenerator? ComponentName { get; set; }

        public Faction Generate(Culture culture, Banner banner, Random random)
        {
            var nameGenerator = new NameGenerator(culture.Language, ComponentName!.Generate(random));
            return new Faction(nameGenerator.GenerateNameForFaction(random), banner, BaseAttributes, nameGenerator);
        }
    }
}
