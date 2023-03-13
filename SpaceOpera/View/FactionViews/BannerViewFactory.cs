using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.FactionViews
{
    public class BannerViewFactory
    {
        public Color4[] Colors { get; set; } = Array.Empty<Color4>();

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
