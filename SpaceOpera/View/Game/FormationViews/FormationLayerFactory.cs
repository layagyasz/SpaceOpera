using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.FormationViews
{
    public class FormationLayerFactory
    {
        public UiElementFactory UiElementFactory { get; }
        public IconFactory IconFactory { get; }

        public FormationLayerFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            UiElementFactory = uiElementFactory;
            IconFactory = iconFactory;
        }

        public FormationLayer<StarSystem> CreateForGalaxy(World? world, Galaxy galaxy, float scale)
        {
            var formationLayer =
                new FormationLayer<StarSystem>(
                    world?.Formations ?? new FormationManager(),
                    new IFormationLayerMapper<StarSystem>.GalaxyMapper(world, galaxy, scale),
                    new FormationSubLayer<StarSystem>(
                        galaxy,
                        UiElementFactory,
                        IconFactory));
            return formationLayer;
        }

        public static FormationLayer<object> CreateForSystem(
            IEnumerable<FormationSubLayer<object>> subLayers, 
            Dictionary<INavigable, Vector3> transitPins, 
            World? world,
            StarSystem starSystem, 
            float scale)
        {
            var formationLayer =
                new FormationLayer<object>(
                    world?.Formations ?? new FormationManager(),
                    new IFormationLayerMapper<object>.StarSystemMapper(world, starSystem, scale, transitPins), 
                    subLayers);
            return formationLayer;
        }

        public FormationSubLayer<object> CreateForTransits(StarSystem starSystem)
        {
            return new(starSystem, UiElementFactory, IconFactory);
        }

        public FormationSubLayer<object> CreateForSubSystem(SolarOrbitRegion region)
        {
            return new(region, UiElementFactory, IconFactory);
        }

        public FormationLayer<INavigable> CreateForStellarBody(
            World? world, StellarBody stellarBody, float surfaceRadius, float atmosphereRadius)
        {
            var formationLayer =
                new FormationLayer<INavigable>(
                    world?.Formations ?? new FormationManager(),
                    new IFormationLayerMapper<INavigable>.StellarBodyMapper(
                        world, stellarBody, surfaceRadius, atmosphereRadius),
                    new FormationSubLayer<INavigable>(stellarBody, UiElementFactory, IconFactory));
            return formationLayer;
        }
    }
}
