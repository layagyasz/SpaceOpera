using SpaceOpera.Core.Languages.Generator;

namespace SpaceOpera.Core.Politics.Generator
{
    public class CultureGenerator
    {
        public LanguageGenerator Language { get; set; }

        public Culture Generate(Random random)
        {
            return new Culture(
                new CulturalTraits()
                {
                    AuthoritarianEgalitarian = GenerateValue(random),
                    IndividualistCollectivist = GenerateValue(random),
                    AggressivePassive = GenerateValue(random),
                    ConventionalDynamic = GenerateValue(random),
                    MonumentalHumble = GenerateValue(random),
                    IndulgentAustere = GenerateValue(random)
                }, 
                Language.Generate(random));
        }

        private static int GenerateValue(Random Random)
        {
            return Random.Next(-2, 3);
        }
    }
}