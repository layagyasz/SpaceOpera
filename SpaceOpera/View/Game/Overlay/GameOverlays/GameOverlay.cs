using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Overlay;
using SpaceOpera.Core;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.Overlay.GameOverlays
{
    public class GameOverlay : DynamicUiCompoundComponent, IOverlay
    {
        public CalendarComponent Calendar { get; }

        private GameOverlay(IController controller, UiSerialContainer container, CalendarComponent calendarOverlay)
            : base(controller, container)
        {
            Calendar = calendarOverlay;
        }

        public void Populate(params object?[] args)
        {
            var world = (World?)args[0];
            Calendar.Populate(world?.Calendar);
        }

        public static GameOverlay Create(UiElementFactory uiElementFactory)
        {
            var calendarOverlay = CalendarComponent.Create(uiElementFactory);
            return new(
                new GameOverlayController(),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass("game-overlay-container"),
                    new ButtonController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-diplomatic-relation"),
                        new ActionButtonController(ActionId.DiplomaticRelation)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-research"),
                        new ActionButtonController(ActionId.Research)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-equipment"),
                        new ActionButtonController(ActionId.Equipment)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-military-organization"),
                        new ActionButtonController(ActionId.MilitaryOrganization)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-military"),
                        new ActionButtonController(ActionId.Military)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("game-overlay-logistics"),
                        new ActionButtonController(ActionId.Logistics)),
                    calendarOverlay
                },
                calendarOverlay);
        }
    }
}
