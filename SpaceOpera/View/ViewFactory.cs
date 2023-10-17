using Cardamom.Collections;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;
using SpaceOpera.View.BannerViews;
using SpaceOpera.View.Components;
using SpaceOpera.View.Game;
using SpaceOpera.View.Game.GalaxyViews;
using SpaceOpera.View.Game.FormationViews;
using SpaceOpera.View.Game.Overlay;
using SpaceOpera.View.Game.Panes;
using SpaceOpera.View.Game.Scenes;
using SpaceOpera.View.Game.StellarBodyViews;
using SpaceOpera.View.Game.StarViews;
using SpaceOpera.View.Icons;
using SpaceOpera.Controller.Game;
using SpaceOpera.View.GameSetup;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.View.Loader;
using SpaceOpera.Core.Loader;
using SpaceOpera.View.Game.Highlights;

namespace SpaceOpera.View
{
    public class ViewFactory
    {
        public UiElementFactory UiElementFactory { get; }
        public SceneFactory SceneFactory { get; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; }
        public BannerViewFactory BannerViewFactory { get; }
        public IconFactory IconFactory { get; set; }

        private ViewFactory(
            UiElementFactory uiElementFactory,
            SceneFactory sceneFactory,
            StellarBodyViewFactory stellarBodyViewFactory,
            BannerViewFactory bannerViewFactory,
            IconFactory iconFactory)
        {
            UiElementFactory = uiElementFactory;
            SceneFactory = sceneFactory;
            StellarBodyViewFactory = stellarBodyViewFactory;
            BannerViewFactory = bannerViewFactory;
            IconFactory = iconFactory;
        }

        public static ViewFactory Create(ViewData viewData, CoreData coreData, LoaderThread loaderThread)
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
                    loaderThread);
            UiElementFactory uiElementFactory = new(viewData.GameResources);
            IconFactory iconFactory =
                new(
                    viewData.Banners!,
                    new(stellarBodyViewFactory),
                    viewData.Icons,
                    viewData.DesignedComponentIconConfigs.ToEnumMap(x => x.ComponentType, x => x),
                    uiElementFactory);
            FormationLayerFactory formationLayerFactory = new(uiElementFactory, iconFactory);
            var highlightShaders =
                new HighlightShaders(
                    viewData.GameResources!.GetShader("shader-default-no-tex"), 
                    viewData.GameResources!.GetShader("shader-border"));
            return new(
                uiElementFactory,
                new(
                    loaderThread,
                    galaxyViewFactory, 
                    stellarBodyViewFactory,
                    new(
                        starViewFactory,
                        stellarBodyViewFactory,
                        formationLayerFactory,
                        viewData.GameResources!.GetShader("shader-transit"),
                        viewData.GameResources!.GetShader("shader-pin"),
                        highlightShaders),
                    starViewFactory,
                    formationLayerFactory,
                    viewData.GameResources!.GetShader("shader-default"),
                    highlightShaders), 
                stellarBodyViewFactory,
                viewData.Banners!,
                iconFactory);
        }

        public GameScreen CreateGameScreen(GameController controller)
        {
            return new(
                controller,
                OverlaySet.Create(UiElementFactory, IconFactory),
                new DynamicUiGroup(new NoOpController()),
                PaneSet.Create(UiElementFactory, IconFactory),
                new DynamicUiGroup(new PaneLayerController()));
        }

        public GameSetupScreen CreateGameSetupScreen(CoreData coreData)
        {
            var screen = new GameSetupScreen(new GameSetupController());
            screen.SetForm(new GameSetupForm(UiElementFactory, IconFactory, coreData, /* random= */ new()));
            return screen;
        }

        public LoaderScreen CreateLoaderScreen(ILoaderTask loaderTask, LoaderStatus loaderStatus)
        {
            return LoaderScreen.Create(UiElementFactory, loaderTask, loaderStatus);
        }
    }
}
