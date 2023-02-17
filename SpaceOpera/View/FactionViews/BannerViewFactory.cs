using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.View.FactionViews
{
    public class BannerViewFactory
    {
        public Color4[] Colors { get; set; } = Array.Empty<Color4>();

        public Color4 GetSecondaryColor(Banner banner)
        {
            return Colors[banner.SecondaryColor];
        }

        public Color4 GetPrimaryColor(Banner banner)
        {
            return Colors[banner.PrimaryColor];
        }
    }
}
