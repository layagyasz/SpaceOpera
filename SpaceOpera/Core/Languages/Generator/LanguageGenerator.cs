using Cardamom.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Languages.Generator
{
    public class LanguageGenerator
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public PhoneticsGenerator? Phonetics { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public OrthographyGenerator? Orthography { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public PhonologyGenerator? Phonology { get; set; }

        public Language Generate(GeneratorContext context)
        {
            var phonetics = Phonetics!.Generate(context);
            return new Language(
                phonetics,
                Orthography!.Generate(phonetics, context),
                Phonology!.Generate(phonetics, context));
        }
    }
}