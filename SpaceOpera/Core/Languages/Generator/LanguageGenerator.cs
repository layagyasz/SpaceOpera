using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Languages.Generator
{
    class LanguageGenerator
    {
        [JsonConverter(typeof(FromFileJsonConverter<PhoneticsGenerator>))]
        public PhoneticsGenerator Phonetics { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter<OrthographyGenerator>))]
        public OrthographyGenerator Orthography { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter<PhonologyGenerator>))]
        public PhonologyGenerator Phonology { get; set; }

        public Language Generate(Random Random)
        {
            var phonetics = Phonetics.Generate(Random);
            return new Language(
                phonetics,
                Orthography.Generate(phonetics, Random),
                Phonology.Generate(phonetics, Random));
        }
    }
}