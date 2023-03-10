using SpaceOpera.Core;
using SpaceOpera.View.FactionViews;

namespace SpaceOpera.View.Common.Highlights
{
    public class FactionHighlight : ICompositeHighlight
    {
        public EventHandler<ValueEventArgs<IHighlight>>? OnHighlightAdded { get; set; }

        public World World { get; }
        public BannerViewFactory BannerViewFactory { get; }

        private readonly List<SingleFactionHighlight> _highlights;

        private FactionHighlight(World world, BannerViewFactory bannerViewFactory)
        {
            World = world;
            BannerViewFactory = bannerViewFactory;
            _highlights = 
                world.GetFactions()
                    .Select(
                        x => new SingleFactionHighlight(
                            x, bannerViewFactory.GetPrimaryColor(x.Banner), bannerViewFactory.GetSecondaryColor(x.Banner)))
                    .ToList();
        }

        public static ICompositeHighlight Create(World world, BannerViewFactory bannerViewFactory)
        {
            return new FactionHighlight(world, bannerViewFactory);
        }

        public IEnumerable<IHighlight> GetHighlights()
        {
            return _highlights;
        }
    }
}