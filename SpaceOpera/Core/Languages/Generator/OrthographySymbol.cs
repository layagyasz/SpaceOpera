namespace SpaceOpera.Core.Languages.Generator
{
    public class OrthographySymbol
    {
        public string Symbol { get; set; } = string.Empty;
        public PhonemeRange Range { get; set; } = PhonemeRange.CreateEmpty();
    }
}
