using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.View.Components;
using Cardamom.Ui.Controller;

namespace SpaceOpera.View.GameSetup
{
    public class CultureComponent : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? SectionHeader { get; set; }
            public string? FieldHeader { get; set; }
            public DialSelect.Style? Select { get; set; }
        }

        private static readonly List<SelectOption<int>> s_AeOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Authoritarian"),
                SelectOption<int>.Create(-1, "Authoritarian"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Egalitarian"),
                SelectOption<int>.Create(2, "Very Egalitarian"),
            };
        private static readonly List<SelectOption<int>> s_IcOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Individualist"),
                SelectOption<int>.Create(-1, "Individualist"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Collectivist"),
                SelectOption<int>.Create(2, "Very Collectivist"),
            };
        private static readonly List<SelectOption<int>> s_ApOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Agressive"),
                SelectOption<int>.Create(-1, "Agressive"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Passive"),
                SelectOption<int>.Create(2, "Very Passive"),
            };
        private static readonly List<SelectOption<int>> s_CdOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Conventional"),
                SelectOption<int>.Create(-1, "Conventional"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Dynamic"),
                SelectOption<int>.Create(2, "Very Dynamic"),
            };
        private static readonly List<SelectOption<int>> s_MhOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Monumental"),
                SelectOption<int>.Create(-1, "Monumental"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Humble"),
                SelectOption<int>.Create(2, "Very Humble"),
            };
        private static readonly List<SelectOption<int>> s_IaOptions =
            new()
            {
                SelectOption<int>.Create(-2, "Very Indulgent"),
                SelectOption<int>.Create(-1, "Indulgent"),
                SelectOption<int>.Create(0, "Moderate"),
                SelectOption<int>.Create(1, "Austere"),
                SelectOption<int>.Create(2, "Very Austere"),
            };

        public IUiComponent AuthoritarianEgalitarian { get; }
        public IUiComponent IndividualistCollectivist { get; }
        public IUiComponent AggressivePassive { get; }
        public IUiComponent ConventionalDynamic { get; }
        public IUiComponent MonumentalHumble { get; }
        public IUiComponent IndulgentAustere { get; }

        public CultureComponent(UiElementFactory uiElementFactory, Style style)
            : base(
                  new CultureComponentController(),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(style.Container!),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            AuthoritarianEgalitarian = DialSelect.Create(uiElementFactory, style.Select!, s_AeOptions, 0);
            IndividualistCollectivist = DialSelect.Create(uiElementFactory, style.Select!, s_IcOptions, 0);
            AggressivePassive = DialSelect.Create(uiElementFactory, style.Select!, s_ApOptions, 0);
            ConventionalDynamic = DialSelect.Create(uiElementFactory, style.Select!, s_CdOptions, 0);
            MonumentalHumble = DialSelect.Create(uiElementFactory, style.Select!, s_MhOptions, 0);
            IndulgentAustere = DialSelect.Create(uiElementFactory, style.Select!, s_IaOptions, 0);

            Add(new TextUiElement(uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Culture"));
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!),
                    new ButtonController(), 
                    "Authoritarian/Egalitarian"));
            Add(AuthoritarianEgalitarian);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!),
                    new ButtonController(), 
                    "Individualist/Collectivist"));
            Add(IndividualistCollectivist);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Aggressive/Passive"));
            Add(AggressivePassive);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Conventional/Dynamic"));
            Add(ConventionalDynamic);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Monumental/Humble"));
            Add(MonumentalHumble);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Indulgent/Austere"));
            Add(IndulgentAustere);
        }
    }
}
