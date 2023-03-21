namespace SpaceOpera.Core.Politics
{
    public class Banner
    {
        public int Symbol { get; }
        public int Pattern { get; }
        public int PrimaryColor { get; }
        public int SecondaryColor { get; }
        public int SymbolColor { get; }

        public Banner(int symbol, int pattern, int primaryColor, int secondaryColor, int symbolColor)
        {
            Symbol = symbol;
            Pattern = pattern;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            SymbolColor = symbolColor;
        }
    }
}