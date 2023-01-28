using Cardamom.Collections;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views
{
    public class ViewFactory
    {
        public StellarBodyViewFactory StellarBodyViewFactory { get; }

        private ViewFactory(StellarBodyViewFactory stellarBodyViewFactory)
        {
            StellarBodyViewFactory = stellarBodyViewFactory;
        }

        public static ViewFactory Create(ViewData viewData, GameData gameData)
        {
            return new(
                new(
                    viewData.BiomeRenderDetails.ToDictionary(x => gameData.Biomes[x.Key], x => x.Value), 
                    gameData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .Select(x => x.Generator!)
                        .ToLibrary(x => x.Key, x => x)));
        }
    }
}
