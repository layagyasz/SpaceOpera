using Cardamom.Ui;
using OpenTK.Mathematics;
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

        public FormationLayer<StarSystem> CreateForGalaxy(World? world, Galaxy galaxy, float scale)
        {
            var formationLayer =
                new FormationLayer<StarSystem>(
                    new IFormationLayerMapper<StarSystem>.GalaxyMapper(world, galaxy, scale),
                    new FormationSubLayer<StarSystem>(
                        galaxy,
                        s_GalaxyOffset,
                        UiElementFactory,
                        IconFactory));
            if (world != null)
            {
                foreach (var fleet in world.GetFleets())
                {
                    formationLayer.Add(fleet);
                }
            }
            return formationLayer;
        }

        public FormationLayer<object> CreateForSystem(
            IEnumerable<FormationSubLayer<object>> subLayers, 
            Dictionary<INavigable, Vector3> transitPins, 
            World? world,
            StarSystem starSystem, 
            float scale)
        {
            var formationLayer =
                new FormationLayer<object>(
                    new IFormationLayerMapper<object>.SubSystemRigMapper(world, starSystem, scale, transitPins), 
                    subLayers);
            if (world != null)
            {
                foreach (var fleet in world.GetFleets())
                {
                    formationLayer.Add(fleet);
                }
            }
            return formationLayer;
        }

        public FormationSubLayer<object> CreateForTransits(StarSystem starSystem)
        {
            return new(starSystem, null, UiElementFactory, IconFactory);
        }

        public FormationSubLayer<object> CreateForSubSystem(SolarOrbitRegion region)
        {
            return new(region, null, UiElementFactory, IconFactory);
        }
    }
}
