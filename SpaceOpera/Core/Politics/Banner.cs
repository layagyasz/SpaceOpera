using OpenTK.Mathematics;

namespace SpaceOpera.Core.Politics
{
    public class Banner
    {
        public uint Symbol { get; }
        public uint Background { get; }
        public Color4 PrimaryColor { get; }
        public Color4 SecondaryColor { get; }
        public Color4 SymbolColor { get; }

        public Banner(uint symbol, uint background, Color4 primaryColor, Color4 secondaryColor, Color4 symbolColor)
        {
            Symbol = symbol;
            Background = background;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            SymbolColor = symbolColor;
        }
    }
}