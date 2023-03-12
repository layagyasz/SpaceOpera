namespace SpaceOpera.Core.Languages
{
    public class OrthographyMatcher
    {
        public Phoneme Pattern { get; }
        public string Symbol { get; }

        public OrthographyMatcher(Phoneme pattern, string symbol)
        {
            Pattern = pattern;
            Symbol = symbol;
        }

        public bool Matches(Phoneme Phoneme)
        {
            return Pattern == Phoneme;
        }

        public override string ToString()
        {
            return $"{Symbol} ({Pattern})";
        }
    }
}