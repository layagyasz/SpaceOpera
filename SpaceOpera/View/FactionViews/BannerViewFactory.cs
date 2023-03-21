using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FactionViews
{
    public class BannerViewFactory
    {
        private static readonly float s_SymbolSize = 0.8f;

        public string SymbolPrefix { get; set; } = string.Empty;
        public string PatternPrefix { get; set; } = string.Empty;
        public string BaseTexture { get; set; } = string.Empty;
        public Color4[] Colors { get; set; } = Array.Empty<Color4>();

        public IEnumerable<IconLayer.Definition> Create(Banner banner)
        {
            var colors = Get(banner);
            yield return new(colors.Primary, BaseTexture);
            yield return new(colors.Secondary, PatternPrefix + banner.Pattern.ToString());
            var min = 0.5f - 0.5f * s_SymbolSize;
            var max = 1 - min;
            yield return new(
                Utils.CreateRect(new(new(min, min), new(max, max))), 
                colors.Symbol, 
                SymbolPrefix + banner.Symbol.ToString());
        }

        public BannerColorSet Get(Banner banner)
        {
            return new(
                Get(banner, BannerColor.Primary), Get(banner, BannerColor.Secondary), Get(banner, BannerColor.Symbol));
        }

        public Color4 Get(Banner banner, BannerColor color)
        {
            return color switch
            {
                BannerColor.Primary => Colors[banner.PrimaryColor],
                BannerColor.Secondary => Colors[banner.SecondaryColor],
                BannerColor.Symbol => Colors[banner.SymbolColor],
                _ => throw new ArgumentException($"Unsupported color {color}"),
            };
        }
    }
}
