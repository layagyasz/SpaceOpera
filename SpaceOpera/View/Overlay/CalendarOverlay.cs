using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Controller;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;

namespace SpaceOpera.View.Overlay
{
    public class CalendarOverlay : UiCompoundComponent, IOverlay
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly TextUiElement _calendarText;

        private StarCalendar? _calendar;

        private CalendarOverlay(
            IController controller, UiSerialContainer container, TextUiElement calendarText)
            : base(controller, container)
        {
            _calendarText = calendarText;
        }

        public void Populate(params object?[] args)
        {
            _calendar = (StarCalendar?)args[0];
        }

        public void Refresh()
        {
            _calendarText.SetText(_calendar?.ToString() ?? string.Empty);
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void SetGameSpeed(ActionId action)
        {
            ((RadioController<ActionId>)ComponentController).SetValue(action);
        }

        public static CalendarOverlay Create(UiElementFactory uiElementFactory)
        {
            var calendarText =
                uiElementFactory.CreateTextButton("overlay-empire-calendar-text", string.Empty).Item1;
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
                (TextUiElement)calendarText);
        }
    }
}
