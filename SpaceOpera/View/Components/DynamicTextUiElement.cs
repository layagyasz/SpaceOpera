using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components
{
    public class DynamicTextUiElement : TextUiElement, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly Func<string> _textFn;

        public DynamicTextUiElement(
            Class @class, IElementController controller, Func<string> textFn) : base(@class, controller, textFn())
        {
            _textFn = textFn;
        }

        public void Refresh()
        {
            SetText(_textFn());
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
