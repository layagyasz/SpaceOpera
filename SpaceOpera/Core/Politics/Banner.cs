namespace SpaceOpera.Core.Politics
{
    public class Banner
    {
        public int Symbol { get; }
        public int Background { get; }
        public int PrimaryColor { get; }
        public int SecondaryColor { get; }
        public int SymbolColor { get; }

        public Banner(int symbol, int background, int primaryColor, int secondaryColor, int symbolColor)
        {
            Symbol = symbol;
            Background = background;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            SymbolColor = symbolColor;
        }
    }
}