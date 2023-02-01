using Cardamom.Collections;
using SpaceOpera.Views.Scenes;
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
            var stellarBodyViewFactory =
                new StellarBodyViewFactory(
                    viewData.BiomeRenderDetails.ToDictionary(x => gameData.Biomes[x.Key], x => x.Value),
                    gameData.GalaxyGenerator!.StarSystemGenerator!.StellarBodySelector!.Options
                        .Select(x => x.Generator!)
                        .ToLibrary(x => x.Key, x => x),
                    viewData.GameResources!.GetShader("shader-light3"),
                    gameData.HumanEyeSensitivity!);
            return new(new(stellarBodyViewFactory, gameData.HumanEyeSensitivity!), stellarBodyViewFactory);
        }
    }
}
