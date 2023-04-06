using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class NumericInput : UiCompoundComponent
    {
        public struct Style
        {
            public string Container { get; set; }
            public string Text { get; set; }
            public string SubtractButton { get; set; }
            public string AddButton { get; set; }
        }

        public TextUiElement Text { get; }
        public IUiElement SubtractButton { get; }
        public IUiElement AddButton { get; }

        private NumericInput(
            Class @class, IController controller, TextUiElement text, IUiElement subtractButton, IUiElement addButton)
            : base(
                  controller, 
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Text = text;
            SubtractButton = subtractButton;
            AddButton = addButton;

            Add(SubtractButton);
            Add(Text);
            Add(AddButton);
        }

        public static NumericInput Create<T>(T key, UiElementFactory uiElementFactory, Style style)
        {
            return new(
                uiElementFactory.GetClass(style.Container), 
                new NumericInputController<T>(key),
                new TextUiElement(uiElementFactory.GetClass(style.Text), new ButtonController(), string.Empty), 
                uiElementFactory.CreateTextButton(style.SubtractButton, "-").Item1, 
                uiElementFactory.CreateTextButton(style.AddButton, "+").Item1);
        }
    }
}
