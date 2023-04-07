using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Controller;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;

namespace SpaceOpera.View.Overlay
{
    public class CalendarOverlay : UiCompoundComponent, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly TextUiElement _calendarText;
        private readonly StarCalendar _calendar;

        private CalendarOverlay(
            IController controller, UiSerialContainer container, TextUiElement calendarText, StarCalendar calendar)
            : base(controller, container)
        {
            _calendarText = calendarText;
            _calendar = calendar;
        }

        public void Refresh()
        {
            _calendarText.SetText(_calendar.ToString());
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void SetGameSpeed(ActionId action)
        {
            ((RadioController<ActionId>)ComponentController).SetValue(action);
        }

        public static CalendarOverlay Create(UiElementFactory uiElementFactory, StarCalendar calendar)
        {
            var calendarText =
                uiElementFactory.CreateTextButton("overlay-empire-calendar-text", calendar.ToString()).Item1;
            return new (
                new RadioController<ActionId>("speed"),
                uiElementFactory.CreateTableRow(
                    "overlay-empire-calendar-container",
                    new List<IUiElement>()
                    {
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-calendar-pause"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedPause)),
                        calendarText,
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-calendar-normal"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedNormal)),
                        new SimpleUiElement(
                            uiElementFactory.GetClass("overlay-empire-calendar-fast"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedFast))
                    },
                    new ButtonController()),
                (TextUiElement)calendarText,
                calendar);
        }
    }
}
