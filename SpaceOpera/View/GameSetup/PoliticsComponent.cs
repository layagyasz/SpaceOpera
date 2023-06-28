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
        private static readonly IntInterval s_States = new(3, 10);
        private static readonly IntInterval s_Cultures = new(3, 20);

        public class Style
        {
            public string? Container { get; set; }
            public string? SectionHeaderContainer { get; set; }
            public string? SectionHeader { get; set; }
            public string? Randomize { get; set; }
            public string? FieldHeader { get; set; }
            public SliderInput.Style? Slider { get; set; }
            public InputWithText.Style? Text { get; set; }
        }

        public IUiElement Randomize { get; }
        public IUiComponent States { get; }
        public IUiComponent Cultures { get; }

        public PoliticsComponent(UiElementFactory uiElementFactory, Style style, Random random)
            : base(
                new PoliticsComponentController(random),
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.Container!),
                new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical))
        {
            Randomize = new SimpleUiElement(uiElementFactory.GetClass(style.Randomize!), new ButtonController());
            States = 
                InputWithText.Create<int>(
                    SliderInput.Create(uiElementFactory, style.Slider!, s_States, 5), uiElementFactory, style.Text!);
            Cultures =
                InputWithText.Create<int>(
                    SliderInput.Create(
                        uiElementFactory, style.Slider!, s_Cultures, 10), uiElementFactory, style.Text!);

            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SectionHeaderContainer!),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Politics"),
                    Randomize
                });
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "States"));
            Add(States);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Cultures"));
            Add(Cultures);
        }
    }
}
