using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class InputWithText : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Text { get; set; }
            public string? Format { get; set; }
        }

        public IUiComponent Input { get; }
        public TextUiElement Text { get; }

        private InputWithText(Class @class, IController controller, IUiComponent input, TextUiElement text)
            : base(
                  controller, 
                  new UiSerialContainer(
                      @class,
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Input = input;
            Text = text;

            Add(Input);
            Add(Text);
        }

        public static InputWithText Create<T>(IUiComponent input, UiElementFactory uiElementFactory, Style style)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                new InputWithTextController<T>(style.Format!),
                input, 
                new TextUiElement(uiElementFactory.GetClass(style.Text!), new InlayController(), string.Empty));
        }
    }
}
