using Cardamom.Collections;
using SpaceOpera.Views.GalaxyViews;
using SpaceOpera.Views.Scenes;
using SpaceOpera.Views.StarViews;
using SpaceOpera.Views.StellarBodyViews;

namespace SpaceOpera.Views
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

        public static ViewFactory Create(ViewData viewData, GameData gameData)
        {
            var starViewFactory = 
                new StarViewFactory(viewData.GameResources!.GetShader("shader-star"), gameData.HumanEyeSensitivity!);
            var galaxyViewFactory = 
                new GalaxyViewFactory(
                    starViewFactory, 
                    viewData.GameResources!.GetShader("shader-default-no-tex"), 
                    viewData.GameResources!.GetShader("shader-pin"));
            var stellarBodyViewFactory =
                new StellarBodyViewFactory(
                    viewData.BiomeRenderDetails.ToDictionary(x => gameData.Biomes[x.Key], x => x.Value),
                    gameData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .Select(x => x.Generator!)
                        .ToLibrary(x => x.Key, x => x),
                    viewData.GameResources!.GetShader("shader-light3"),
                    viewData.GameResources!.GetShader("shader-atmosphere"),
                    gameData.HumanEyeSensitivity!); ;
            return new(
                new(
                    galaxyViewFactory, 
                    stellarBodyViewFactory,
                    starViewFactory, 
                    gameData.HumanEyeSensitivity!,
                    viewData.GameResources!.GetShader("shader-default")), 
                stellarBodyViewFactory);
        }
    }
}
