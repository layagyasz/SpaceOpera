using OpenTK.Mathematics;

namespace SpaceOpera.View.BannerViews
{
    public struct BannerColorSet
    {
        public static readonly BannerColorSet Default = new(Color4.White, Color4.Black, Color4.Red);

        public Color4 Primary { get; set; }
        public Color4 Secondary { get; set; }
        public Color4 Symbol { get; set; }

        public BannerColorSet(Color4 primary, Color4 secondary, Color4 symbol)
        {
            Primary = primary;
            Secondary = secondary;
            Symbol = symbol;
        }

        public Color4 Get(BannerColor color)
        {
            return color switch
            {
                BannerColor.Primary => Primary,
                BannerColor.Secondary => Secondary,
                BannerColor.Symbol => Symbol,
                _ => throw new ArgumentException($"Unsupported color {color}"),
            };
        }
    }
}
