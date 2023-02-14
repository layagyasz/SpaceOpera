using SpaceOpera.Core;
using SpaceOpera.View.FactionViews;

namespace SpaceOpera.View.Scenes.Highlights
{
    public class FactionHighlight : ICompositeHighlight
    {
        public EventHandler<ElementEventArgs<IHighlight>>? OnHighlightAdded { get; set; }

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
                            x, bannerViewFactory.GetForeground(x.Banner), bannerViewFactory.GetBackground(x.Banner)))
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