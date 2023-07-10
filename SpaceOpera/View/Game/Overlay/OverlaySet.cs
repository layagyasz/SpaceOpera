using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Game.Overlay.EmpireOverlays;
using SpaceOpera.View.Game.Overlay.GameOverlays;
using SpaceOpera.View.Game.Overlay.StarSystemOverlays;
using Cardamom.Graphics;

namespace SpaceOpera.View.Game.Overlay
{
    public class OverlaySet : GraphicsResource
    {
        public EmpireOverlay Empire { get; }
        public GameOverlay Game { get; }
        public StarSystemOverlay StarSystem { get; }

        private OverlaySet(EmpireOverlay empire, GameOverlay game, StarSystemOverlay starSystem)
        {
            Empire = empire;
            Game = game;
            StarSystem = starSystem;
        }

        public IOverlay Get(OverlayId id)
        {
            return id switch
            {
                OverlayId.Empire => Empire,
                OverlayId.Game => Game,
                OverlayId.StarSystem => StarSystem,
                _ => throw new ArgumentException($"Unsupported overlay id: {id}"),
            };
        }

        public IEnumerable<IOverlay> GetOverlays()
        {
            yield return Empire;
            yield return Game;
            yield return StarSystem;
        }

        protected override void DisposeImpl()
        {
            foreach (var overlay in GetOverlays())
            {
                overlay.Dispose();
            }
        }

        public static OverlaySet Create(UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var empire = new EmpireOverlay(uiElementFactory, iconFactory);
            empire.Initialize();

            var game = GameOverlay.Create(uiElementFactory);
            game.Initialize();

            var starSystem = new StarSystemOverlay(uiElementFactory, iconFactory);
            starSystem.Initialize();

            return new(empire, game, starSystem);
        }
    }
}
