using Cardamom.Collections;
using SpaceOpera.Core;
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

        private ViewFactory(SceneFactory sceneFactory, StellarBodyViewFactory stellarBodyViewFactory)
        {
            SceneFactory = sceneFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
        }

        public static ViewFactory Create(ViewData viewData, CoreData coreData)
        {
            var starViewFactory = 
                new StarViewFactory(viewData.GameResources!.GetShader("shader-star"), viewData.HumanEyeSensitivity!);
            var galaxyViewFactory = 
                new GalaxyViewFactory(
                    starViewFactory, 
                    viewData.GameResources!.GetShader("shader-default-no-tex"), 
                    viewData.GameResources!.GetShader("shader-pin"));
            var stellarBodyViewFactory =
                new StellarBodyViewFactory(
                    viewData.BiomeRenderDetails.ToDictionary(x => coreData.Biomes[x.Key], x => x.Value),
                    coreData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .Select(x => x.Generator!)
                        .ToLibrary(x => x.Key, x => x),
                    viewData.GameResources!.GetShader("shader-light3"),
                    viewData.GameResources!.GetShader("shader-atmosphere"),
                    viewData.HumanEyeSensitivity!); ;
            return new(
                new(
                    galaxyViewFactory, 
                    stellarBodyViewFactory,
                    starViewFactory,
                    viewData.HumanEyeSensitivity!,
                    viewData.GameResources!.GetShader("shader-default")), 
                stellarBodyViewFactory);
        }
    }
}
