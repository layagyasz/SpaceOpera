using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class DialSelect : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Text { get; set; }
            public string? LeftButton { get; set; }
            public string? RightButton { get; set; }
        }

        public TextUiElement Text { get; }
        public IUiElement LeftButton { get; }
        public IUiElement RightButton { get; }  

        private DialSelect(
            Class @class, IController controller, TextUiElement text, IUiElement leftButton, IUiElement rightButton)
            : base(
                  controller,
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Text = text;
            LeftButton = leftButton;
            RightButton = rightButton;

            Add(LeftButton);
            Add(Text);
            Add(RightButton);
        }

        public static DialSelect Create<T>(
            UiElementFactory uiElementFactory, 
            Style style,
            IEnumerable<SelectOption<T>> range,
            T initialValue, bool wrap = false)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                new DialSelectController<T>(range, initialValue, wrap),
                new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), string.Empty),
                new TextUiElement(uiElementFactory.GetClass(style.LeftButton!), new ButtonController(), "<"),
                new TextUiElement(uiElementFactory.GetClass(style.RightButton!), new ButtonController(), ">"));
        }
    }
}
