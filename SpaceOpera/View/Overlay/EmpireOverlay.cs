using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Overlay;
using SpaceOpera.Core;

namespace SpaceOpera.View.Overlay
{
    public class EmpireOverlay : UiCompoundComponent, IDynamic
    {
        public CalendarOverlay CalendarOverlay { get; }
        
        private EmpireOverlay(IController controller, UiSerialContainer container, CalendarOverlay calendarOverlay)
            : base(controller, container)
        {
            CalendarOverlay = calendarOverlay;
        }

        public void Refresh()
        {
            foreach (var item in this)
            {
                if (item is IDynamic dynamic)
                {
                    dynamic.Refresh();
                }
            }
        }

        public static EmpireOverlay Create(UiElementFactory uiElementFactory, StarCalendar calendar)
        {
            var calendarOverlay = CalendarOverlay.Create(uiElementFactory, calendar);
            return new(
                new EmpireOverlayController(),
                uiElementFactory.CreateTableRow(
                    "overlay-empire-container", 
                    new List<IUiElement>()
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
                    new ButtonController()),
                calendarOverlay);
        }
    }
}
