using SpaceOpera.Core.Universe;
using SpaceOpera.Views.StarViews;

namespace SpaceOpera.Views.GalaxyViews
{
    public class GalaxyViewFactory
    {
        private static readonly float s_StarScale = 5000;

        public StarViewFactory StarViewFactory { get; }

        public GalaxyViewFactory(StarViewFactory starViewFactory)
        {
            StarViewFactory = starViewFactory;
        }

        public GalaxyModel CreateModel(Galaxy galaxy)
        {
            var positionFactor = 1f / galaxy.Radius;
            return new GalaxyModel(
                StarViewFactory.CreateView(galaxy.Systems.Select(x => x.Star),
                galaxy.Systems.Select(x => positionFactor * x.Position),
                s_StarScale * positionFactor));
        }
    }
}
