using Cardamom.Ui;
using SpaceOpera.Core;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationLayerFactory
    {
        private static readonly float s_GalaxyOffset = 32f;

        public UiElementFactory UiElementFactory { get; }
        public IconFactory IconFactory { get; }

        public FormationLayerFactory(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            UiElementFactory = uiElementFactory;
            IconFactory = iconFactory;
        }

        public FormationLayer<StarSystem>? CreateForGalaxy(World? world, float scale)
        {
            if (world == null)
            {
                return null;
            }
            var formationLayer =
                new FormationLayer<StarSystem>(
                    new IFormationLayerMapper<StarSystem>.GalaxyMapper(world, scale),
                    s_GalaxyOffset,
                    UiElementFactory,
                    IconFactory);
            foreach (var fleet in world.GetFleets())
            {
                formationLayer.Add(fleet);
            }
            return formationLayer;
        }

        public FormationLayer<object>? CreateForSubSystem(World? world, SolarOrbitRegion region, float scale)
        {
            if (world == null)
            {
                return null;
            }
            var formationLayer =
                new FormationLayer<object>(
                    new IFormationLayerMapper<object>.SubSystemRigMapper(world, region, scale),
                    null,
                    UiElementFactory,
                    IconFactory);
            foreach (var fleet in world.GetFleets())
            {
                formationLayer.Add(fleet);
            }
            return formationLayer;
        }
    }
}
