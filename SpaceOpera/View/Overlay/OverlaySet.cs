using Cardamom.Ui;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Overlay
{
    public class OverlaySet
    {
        public EmpireOverlay Empire { get; }
        public StarSystemOverlay StarSystem { get; }

        private OverlaySet(EmpireOverlay empire, StarSystemOverlay starSystem)
        {
            Empire = empire;
            StarSystem = starSystem;
        }

        public IOverlay Get(OverlayId id)
        {
            return id switch
            {
                OverlayId.Empire => Empire,
                OverlayId.StarSystem => StarSystem,
                _ => throw new ArgumentException($"Unsupported overlay id: {id}"),
            };
        }

        public IEnumerable<IOverlay> GetOverlays()
        {
            yield return Empire;
            yield return StarSystem;
        }

        public static OverlaySet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var empire = EmpireOverlay.Create(uiElementFactory);
            empire.Initialize();

            var starSystem = new StarSystemOverlay(uiElementFactory, iconFactory);
            starSystem.Initialize();

            return new(empire, starSystem);
        }
    }
}
