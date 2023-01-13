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

        public Language Generate(Random Random)
        {
            var phonetics = Phonetics!.Generate(Random);
            return new Language(
                phonetics,
                Orthography!.Generate(phonetics, Random),
                Phonology!.Generate(phonetics, Random));
        }
    }
}