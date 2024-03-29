﻿using Cardamom.Ui;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Game.Overlay.EmpireOverlays;
using SpaceOpera.View.Game.Overlay.GameOverlays;
using SpaceOpera.View.Game.Overlay.StarSystemOverlays;
using Cardamom.Graphics;
using Cardamom;
using SpaceOpera.View.Game.Overlay.EventOverlays;

namespace SpaceOpera.View.Game.Overlay
{
    public class OverlaySet : GraphicsResource, IInitializable
    {
        public EmpireOverlay Empire { get; }
        public EventOverlay Event { get; }
        public GameOverlay Game { get; }
        public StarSystemOverlay StarSystem { get; }

        private OverlaySet(EmpireOverlay empire, EventOverlay @event, GameOverlay game, StarSystemOverlay starSystem)
        {
            Empire = empire;
            Event = @event;
            Game = game;
            StarSystem = starSystem;
        }

        public IOverlay Get(OverlayId id)
        {
            return id switch
            {
                OverlayId.Empire => Empire,
                OverlayId.Event => Event,
                OverlayId.Game => Game,
                OverlayId.StarSystem => StarSystem,
                _ => throw new ArgumentException($"Unsupported overlay id: {id}"),
            };;
        }

        public IEnumerable<IOverlay> GetOverlays()
        {
            yield return Empire;
            yield return Event;
            yield return Game;
            yield return StarSystem;
        }

        public void Initialize()
        {
            foreach (var overlay in GetOverlays())
            {
                overlay.Initialize();
            }
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
            return new(
                /* empire= */ new EmpireOverlay(uiElementFactory, iconFactory),
                /* event= */ new EventOverlay(uiElementFactory, iconFactory),
                /* game= */ GameOverlay.Create(uiElementFactory),
                /* starSystem= */ new StarSystemOverlay(uiElementFactory, iconFactory));
        }
    }
}
