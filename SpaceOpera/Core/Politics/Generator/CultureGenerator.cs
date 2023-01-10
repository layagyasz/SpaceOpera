using SpaceOpera.Core.Languages;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics.Generator
{
    class CultureGenerator
    {
        public LanguageGenerator Language { get; set; }

        public Culture Generate(Random Random)
        {
            return new Culture(
                new CulturalTraits()
                {
                    AuthoritarianEgalitarian = GenerateValue(Random),
                    IndividualistCollectivist = GenerateValue(Random),
                    AggressivePassive = GenerateValue(Random),
                    ConventionalDynamic = GenerateValue(Random),
                    MonumentalHumble = GenerateValue(Random),
                    IndulgentAustere = GenerateValue(Random)
                }, 
                Language.Generate(Random));
        }

        private static int GenerateValue(Random Random)
        {
            return Random.Next(-2, 3);
        }
    }
}