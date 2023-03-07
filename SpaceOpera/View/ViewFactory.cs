using Cardamom.Collections;
using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller;
using SpaceOpera.Core;
using SpaceOpera.View.FactionViews;
using SpaceOpera.View.GalaxyViews;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Overlay;
using SpaceOpera.View.Panes;
using SpaceOpera.View.Scenes;
using SpaceOpera.View.StarViews;
using SpaceOpera.View.StellarBodyViews;

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
            UiElementFactory uiElementFactory = new(viewData.GameResources);
            return new(
                uiElementFactory,
                new(
                    galaxyViewFactory, 
                    stellarBodyViewFactory,
                    new(
                        starViewFactory,
                        stellarBodyViewFactory,
                        viewData.GameResources!.GetShader("shader-transit"),
                        viewData.GameResources!.GetShader("shader-border"),
                        viewData.GameResources!.GetShader("shader-default-no-tex"),
                        viewData.GameResources!.GetShader("shader-pin")),
                    starViewFactory,
                    viewData.HumanEyeSensitivity!,
                    viewData.GameResources!.GetShader("shader-default"),
                    viewData.GameResources!.GetShader("shader-border"),
                    viewData.GameResources!.GetShader("shader-default-no-tex")), 
                stellarBodyViewFactory,
                viewData.Banners!,
                new(
                    viewData.Icons, 
                    viewData.DesignedComponentIconConfigs.ToEnumMap(x => x.ComponentType, x => x),
                    uiElementFactory));
        }

        public GameScreen CreateGameScreen(GameController controller)
        {
            return new(
                controller,
                EmpireOverlay.Create(UiElementFactory),
                PaneSet.Create(UiElementFactory),
                new UiGroup(new PaneLayerController()));
        }
    }
}
