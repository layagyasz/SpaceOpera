namespace SpaceOpera.Core.Languages
{
    public class Phoneme
    {
        public string Symbol { get; set; } = string.Empty;
        public PhonemeRange Range { get; set; } = PhonemeRange.CreateEmpty();


        public override string ToString()
        {
            return Symbol;
        }
    }
}
