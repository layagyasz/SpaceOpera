using Cardamom.Mathematics;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;
using SpaceOpera.Controller.GameSetup;

namespace SpaceOpera.View.GameSetup
{
    public class PoliticsComponent : UiCompoundComponent
    {
        private static readonly IntInterval s_States = new(1, 10);
        private static readonly IntInterval s_Cultures = new(1, 20);

        public class Style
        {
            public string? Container { get; set; }
            public string? SectionHeader { get; set; }
            public string? FieldHeader { get; set; }
            public SliderInput.Style? Slider { get; set; }
            public InputWithText.Style? Text { get; set; }
        }

        public IUiComponent States { get; }
        public IUiComponent Cultures { get; }

        public PoliticsComponent(UiElementFactory uiElementFactory, Style style)
            : base(
                new PoliticsComponentController(),
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Container!),
                new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical))
        {
            States = 
                InputWithText.Create<int>(
                    SliderInput.Create(s_States, uiElementFactory, style.Slider!), uiElementFactory, style.Text!);
            Cultures =
                InputWithText.Create<int>(
                    SliderInput.Create(s_Cultures, uiElementFactory, style.Slider!), uiElementFactory, style.Text!);

            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Politics"));
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "States"));
            Add(States);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Cultures"));
            Add(Cultures);
        }
    }
}
