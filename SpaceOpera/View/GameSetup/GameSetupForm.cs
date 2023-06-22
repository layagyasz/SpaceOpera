using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Politics.Generator;
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

        private static readonly BannerComponent.Style s_BannerStyle =
            new()
            {
                Container = "game-setup-form-banner",
                SectionHeader = "game-setup-form-section-header",
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
                SectionHeader = "game-setup-form-section-header",
                FieldHeader = "game-setup-form-field-header",
                Select = s_DialStyle
            };

        private static readonly GalaxyComponent.Style s_GalaxyStyle =
            new()
            {
                Container = "game-setup-form-galaxy",
                SectionHeader = "game-setup-form-section-header",
                FieldHeader = "game-setup-form-field-header",
                Select = s_DialStyle
            };

        public IUiComponent Banner { get; }
        public IUiComponent Culture { get; }
        public IUiComponent Galaxy { get; }

        public GameSetupForm(
            UiElementFactory uiElementFactory, IconFactory iconFactory, BannerGenerator bannerGenerator)
            : base(
                  new NoOpController<GameSetupForm>(), 
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

            Banner = new BannerComponent(uiElementFactory, iconFactory, s_BannerStyle, bannerGenerator);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column), 
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Banner
                });

            Culture = new CultureComponent(uiElementFactory, s_CultureStyle);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Culture
                });

            Galaxy = new GalaxyComponent(uiElementFactory, s_GalaxyStyle);
            body.Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_Column),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical)
                {
                    Galaxy
                });

            Add(body);
        }
    }
}
