using SpaceOpera.Core.Languages;

namespace SpaceOpera.Core.Politics.Cultures
{
    public class Culture
    {
        public CulturalTraits Traits { get; }
        public Language Language { get; }

        public Culture(CulturalTraits traits, Language language)
        {
            Traits = traits;
            Language = language;
        }
    }
}
