using SpaceOpera.Core;
using SpaceOpera.View.BannerViews;

namespace SpaceOpera.View.Game.Highlights
{
    public class FactionHighlight : ICompositeHighlight
    {
        public EventHandler<IHighlight>? HighlightAdded { get; set; }

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
                            x,
                            bannerViewFactory.Get(x.Banner, BannerColor.Symbol),
                            bannerViewFactory.Get(x.Banner, BannerColor.Primary)))
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

        public void Hook(object domain)
        {
            _highlights.ForEach(x => x.Hook(domain));
        }

        public void Unhook(object domain)
        {
            _highlights.ForEach(x => x.Unhook(domain));
        }
    }
}