using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Politics.Governments;

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
            public Select.Style? Select { get; set; }
        }

        public IUiElement Randomize { get; }
        public EditableTextUiElement Name { get; }
        public IUiComponent Government { get; }

        public GovernmentComponent(
            UiElementFactory uiElementFactory, 
            Style style,
            FactionGenerator factionGenerator,
            LanguageGenerator languageGenerator,
            Random random)
            : base(
                  new GovernmentComponentController(factionGenerator, languageGenerator, random),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(style.Container!), 
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Randomize = new SimpleUiElement(uiElementFactory.GetClass(style.Randomize!), new ButtonController());
            Name = 
                new EditableTextUiElement(
                    uiElementFactory.GetClass(style.Text!), new TextInputController(), string.Empty);
            Government =
                uiElementFactory.CreateSelect(
                    style.Select!, new List<SelectOption<GovernmentForm>>(), scrollSpeed: 10f).Item1;

            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SectionHeaderContainer!),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Government"),
                    Randomize
                });
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Name"));
            Add(Name);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Type"));
            Add(Government);
        }
    }
}
