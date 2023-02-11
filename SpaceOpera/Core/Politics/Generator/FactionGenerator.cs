using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Generator
{
    public class FactionGenerator
    {
        public EnumMap<FactionAttribute, float> BaseAttributes { get; set; } = new();
        public ComponentNameGeneratorGenerator? ComponentName { get; set; }

        public Faction Generate(Culture culture, Banner banner, GeneratorContext context)
        {
            var nameGenerator = new NameGenerator(culture.Language, ComponentName!.Generate(context));
            return new Faction(
                nameGenerator.GenerateNameForFaction(context.Random), banner, BaseAttributes, nameGenerator);
        }
    }
}
