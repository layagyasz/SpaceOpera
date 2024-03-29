﻿using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.BannerViews
{
    public class BannerViewFactory
    {
        private static readonly float s_SymbolSize = 0.75f;

        public string SymbolPrefix { get; set; } = string.Empty;
        public string PatternPrefix { get; set; } = string.Empty;
        public string BaseTexture { get; set; } = string.Empty;

        public IEnumerable<IconLayer> Create(Banner banner)
        {
            var colors = Get(banner);
            yield return new(null, colors.Primary, BaseTexture, false);
            yield return new(null, colors.Secondary, PatternPrefix + banner.Pattern.ToString(), false);
            var min = 0.5f - 0.5f * s_SymbolSize;
            var max = 1 - min;
            yield return new(
                Utils.CreateRect(new(new(min, min), new(max, max))), 
                colors.Symbol, 
                SymbolPrefix + banner.Symbol.ToString(),
                false);
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
                BannerColor.Primary => banner.PrimaryColor,
                BannerColor.Secondary => banner.SecondaryColor,
                BannerColor.Symbol => banner.SymbolColor,
                _ => throw new ArgumentException($"Unsupported color {color}"),
            };
        }
    }
}
