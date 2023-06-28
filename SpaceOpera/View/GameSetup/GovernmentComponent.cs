using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.Core.Politics.Generator;

namespace SpaceOpera.View.GameSetup
{
    public class GovernmentComponent : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? SectionHeaderContainer { get; set; }
            public string? SectionHeader { get; set; }
            public string? Randomize { get; set; }
            public string? FieldHeader { get; set; }
            public string? Text { get; set; }
        }

        public IUiElement Randomize { get; }
        public EditableTextUiElement Name { get; }

        public GovernmentComponent(
            UiElementFactory uiElementFactory, 
            Style style, 
            LanguageGenerator languageGenerator,
            ComponentNameGeneratorGenerator nameGeneratorGenerator,
            Random random)
            : base(
                  new GovernmentComponentController(languageGenerator, nameGeneratorGenerator, random),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(style.Container!), 
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Randomize = new SimpleUiElement(uiElementFactory.GetClass(style.Randomize!), new ButtonController());
            Name = 
                new EditableTextUiElement(
                    uiElementFactory.GetClass(style.Text!), new TextInputController(), string.Empty);

            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SectionHeaderContainer!),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Government"),
                    Randomize
                });
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Name"));
            Add(Name);
        }
    }
}
