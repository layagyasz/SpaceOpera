using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Core;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.GameSetup
{
    public class GameSetupForm : UiCompoundComponent
    {
        private static readonly string s_Container = "game-setup-form";
        private static readonly string s_Header = "game-setup-form-title";
        private static readonly string s_Body = "game-setup-form-body";
        private static readonly string s_Column = "game-setup-form-column";

        private static readonly DialSelect.Style s_DialStyle =
            new()
            {
                Container = "game-setup-form-dial",
                Text = "game-setup-form-dial-text",
                LeftButton = "game-setup-form-dial-left-button",
                RightButton = "game-setup-form-dial-right-button"
            };

        private static readonly ColorSelect.Style s_ColorStyle =
            new()
            {
                Root = "game-setup-form-color",
                OptionContainer = "game-setup-form-color-option-container",
                OptionRow = "game-setup-form-color-option-row",
                Option = "game-setup-form-color-option",
                OptionsRowSize = 5
            };

        private static readonly SliderInput.Style s_SliderStyle =
            new()
            {
                Track = "game-setup-form-slider-track",
                Knob = "game-setup-form-slider-knob"
            };

        private static readonly InputWithText.Style s_SliderTextStyle =
            new()
            {
                Container = "game-setup-form-slider",
                Text = "game-setup-form-slider-text",
                Format = "{0:N0}"
            };

        private static readonly BannerComponent.Style s_BannerStyle =
            new()
            {
                Container = "game-setup-form-banner",
                SectionHeaderContainer = "game-setup-form-section-header-container",
                SectionHeader = "game-setup-form-section-header",
                Randomize = "game-setup-form-randomize",
                FieldHeader = "game-setup-form-field-header",
                Banner = "game-setup-form-banner-icon",
                Symbol = s_DialStyle,
                Pattern = s_DialStyle,
                PrimaryColor = s_ColorStyle,
                SecondaryColor = s_ColorStyle,
                SymbolColor = s_ColorStyle
            };

        private static readonly CultureComponent.Style s_CultureStyle =
            new()
            {
                Container = "game-setup-form-culture",
                SectionHeaderContainer = "game-setup-form-section-header-container",
                SectionHeader = "game-setup-form-section-header",
                Randomize = "game-setup-form-randomize",
                FieldHeader = "game-setup-form-field-header",
                Select = s_DialStyle
            };

        private static readonly GalaxyComponent.Style s_GalaxyStyle =
            new()
            {
                Container = "game-setup-form-galaxy",
                SectionHeaderContainer = "game-setup-form-section-header-container",
                SectionHeader = "game-setup-form-section-header",
                Randomize = "game-setup-form-randomize",
                FieldHeader = "game-setup-form-field-header",
                Select = s_DialStyle
            };

        private static readonly GovernmentComponent.Style s_GovernmentStyle =
            new()
            {
                Container = "game-setup-form-government",
                SectionHeaderContainer = "game-setup-form-section-header-container",
                SectionHeader = "game-setup-form-section-header",
                Randomize = "game-setup-form-randomize",
                FieldHeader = "game-setup-form-field-header",
                Text = "game-setup-form-text"
            };

        private static readonly PoliticsComponent.Style s_PoliticsStyle =
            new()
            {
                Container = "game-setup-form-politics",
                SectionHeaderContainer = "game-setup-form-section-header-container",
                SectionHeader = "game-setup-form-section-header",
                Randomize = "game-setup-form-randomize",
                FieldHeader = "game-setup-form-field-header",
                Slider = s_SliderStyle,
                Text = s_SliderTextStyle
            };

        private static readonly string s_Start = "game-setup-form-start";

        public IUiComponent Banner { get; }
        public IUiComponent Culture { get; }
        public IUiComponent Galaxy { get; }
        public IUiComponent Government { get; }
        public IUiComponent Politics { get; }
        public IUiElement Start { get; }

        public GameSetupForm(
            UiElementFactory uiElementFactory, IconFactory iconFactory, CoreData coreData, Random random)
            : base(
                  new GameSetupFormController(coreData.PoliticsGenerator!.Faction!, random), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new ButtonController(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Add(uiElementFactory.CreateTextButton(s_Header, "Game Setup").Item1);

            var body = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal);

            Banner = 
                new BannerComponent(
                    uiElementFactory, iconFactory, s_BannerStyle, coreData.PoliticsGenerator!.Banner!, random);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column), 
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Banner
                });

            Government = 
                new GovernmentComponent(
                    uiElementFactory,
                    s_GovernmentStyle,
                    coreData.PoliticsGenerator!.Culture!.Language!,
                    coreData.PoliticsGenerator!.Faction!.ComponentName!,
                    random);
            Culture = new CultureComponent(uiElementFactory, s_CultureStyle, random);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Government,
                    Culture
                });

            Galaxy = new GalaxyComponent(uiElementFactory, s_GalaxyStyle, random);
            Politics = new PoliticsComponent(uiElementFactory, s_PoliticsStyle, random);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Galaxy,
                    Politics
                });

            Start = new TextUiElement(uiElementFactory.GetClass(s_Start), new ButtonController(), "Start");
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Start
                });

            Add(body);
        }
    }
}
