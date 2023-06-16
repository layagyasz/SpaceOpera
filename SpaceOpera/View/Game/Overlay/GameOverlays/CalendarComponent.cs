using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Controller;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Core;
using SpaceOpera.View.Game;

namespace SpaceOpera.View.Game.Overlay.GameOverlays
{
    public class CalendarComponent : UiCompoundComponent, IOverlay
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly TextUiElement _calendarText;

        private StarCalendar? _calendar;

        private CalendarComponent(
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

        public static CalendarComponent Create(UiElementFactory uiElementFactory)
        {
            var calendarText =
                uiElementFactory.CreateTextButton("game-overlay-calendar-text", string.Empty).Item1;
            return new(
                new RadioController<ActionId>(),
                uiElementFactory.CreateTableRow(
                    "game-overlay-calendar-container",
                    new List<IUiElement>()
                    {
                        new SimpleUiElement(
                            uiElementFactory.GetClass("game-overlay-calendar-pause"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedPause)),
                        calendarText,
                        new SimpleUiElement(
                            uiElementFactory.GetClass("game-overlay-calendar-normal"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedNormal)),
                        new SimpleUiElement(
                            uiElementFactory.GetClass("game-overlay-calendar-fast"),
                            new OptionElementController<ActionId>(ActionId.GameSpeedFast))
                    },
                    new ButtonController()),
                (TextUiElement)calendarText);
        }
    }
}
