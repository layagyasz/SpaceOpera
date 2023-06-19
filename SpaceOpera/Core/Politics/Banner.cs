using OpenTK.Mathematics;

namespace SpaceOpera.Core.Politics
{
    public class Banner
    {
        public int Symbol { get; }
        public int Pattern { get; }
        public Color4 PrimaryColor { get; }
        public Color4 SecondaryColor { get; }
        public Color4 SymbolColor { get; }

        public Banner(int symbol, int pattern, Color4 primaryColor, Color4 secondaryColor, Color4 symbolColor)
        {
            Symbol = symbol;
            Pattern = pattern;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            SymbolColor = symbolColor;
        }
    }
}