using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Overlay;
using SpaceOpera.Core;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Overlay
{
    public class EmpireOverlay : DynamicUiCompoundComponent, IDynamic
    {
        public CalendarOverlay CalendarOverlay { get; }
        
        private EmpireOverlay(IController controller, UiSerialContainer container, CalendarOverlay calendarOverlay)
            : base(controller, container)
        {
            CalendarOverlay = calendarOverlay;
        }

        public static EmpireOverlay Create(UiElementFactory uiElementFactory, StarCalendar calendar)
        {
            var calendarOverlay = CalendarOverlay.Create(uiElementFactory, calendar);
            return new(
                new EmpireOverlayController(),
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass("overlay-empire-container"), 
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new SimpleUiElement(
                        uiElementFactory.GetClass("overlay-empire-research"),
                        new ActionButtonController(ActionId.Research)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("overlay-empire-equipment"),
                        new ActionButtonController(ActionId.Equipment)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("overlay-empire-military-organization"),
                        new ActionButtonController(ActionId.MilitaryOrganization)),
                    new SimpleUiElement(
                        uiElementFactory.GetClass("overlay-empire-military"),
                        new ActionButtonController(ActionId.Military)),
                    calendarOverlay
                },
                calendarOverlay);
        }
    }
}
