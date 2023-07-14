using Cardamom.Collections;
using Cardamom.Json.Collections;
using SpaceOpera.Core.Politics.Government;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Generator
{
    public class FactionGenerator
    {
        public EnumMap<FactionAttribute, float> BaseAttributes { get; set; } = new();
        public ComponentNameGeneratorGenerator? ComponentName { get; set; }

        [JsonConverter(typeof(ReferenceCollectionJsonConverter))]
        public List<GovernmentForm> GovernmentForms { get; set; } = new();

        public Faction Generate(string name, Banner banner, GovernmentForm governmentForm, NameGenerator nameGenerator)
        {
            return new(name, banner, governmentForm, BaseAttributes, nameGenerator);
        }

        public Faction Generate(Culture culture, Banner banner, GeneratorContext context)
        {
            var nameGenerator = new NameGenerator(culture.Language, ComponentName!.Generate(context));
            return new Faction(
                nameGenerator.GenerateNameForFaction(context.Random),
                banner, 
                GovernmentForms[context.Random.Next(0, GovernmentForms.Count)],
                BaseAttributes, 
                nameGenerator);
        }
    }
}
