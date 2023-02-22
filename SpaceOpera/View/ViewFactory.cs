using Cardamom.Collections;
using Cardamom.Logging;
using SpaceOpera.Core;
using SpaceOpera.View.FactionViews;
using SpaceOpera.View.GalaxyViews;
using SpaceOpera.View.Scenes;
using SpaceOpera.View.StarViews;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera.View
{
    public class ViewFactory
    {
        public SceneFactory SceneFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public BannerViewFactory BannerViewFactory { get; }

        private ViewFactory(
            SceneFactory sceneFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            BannerViewFactory bannerViewFactory)
        {
            SceneFactory = sceneFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            BannerViewFactory = bannerViewFactory;
        }

        public static ViewFactory Create(ViewData viewData, CoreData coreData, ILogger logger)
        {
            var starViewFactory = 
                new StarViewFactory(viewData.GameResources!.GetShader("shader-star"), viewData.HumanEyeSensitivity!);
            var galaxyViewFactory = 
                new GalaxyViewFactory(
                    starViewFactory,
                    viewData.GameResources!.GetShader("shader-transit"), 
                    viewData.GameResources!.GetShader("shader-pin"));
            var stellarBodyViewFactory =
                new StellarBodyViewFactory(
                    viewData.Biomes.ToDictionary(x => coreData.Biomes[x.Key], x => x.Value),
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .Select(x => x.Generator!)
                        .ToLibrary(x => x.Key, x => x),
                    viewData.GameResources!.GetShader("shader-light3"),
                    viewData.GameResources!.GetShader("shader-atmosphere"),
                    viewData.HumanEyeSensitivity!,
                    logger);
            return new(
                new(
                    galaxyViewFactory, 
                    stellarBodyViewFactory,
                    new(
                        viewData.GameResources!.GetShader("shader-border"),
                        viewData.GameResources!.GetShader("shader-default-no-tex"),
                        viewData.GameResources!.GetShader("shader-pin")),
                    starViewFactory,
                    viewData.HumanEyeSensitivity!,
                    viewData.GameResources!.GetShader("shader-default"),
                    viewData.GameResources!.GetShader("shader-border"),
                    viewData.GameResources!.GetShader("shader-default-no-tex"),
                    viewData.GameResources!.GetShader("shader-transit")), 
                stellarBodyViewFactory,
                viewData.Banners!);
        }
    }
}
