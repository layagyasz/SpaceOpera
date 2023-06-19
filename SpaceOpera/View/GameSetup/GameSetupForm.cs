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

        private static readonly DialSelect.Style s_DialStyle =
            new()
            {
                Container = "game-setup-form-dial",
                Text = "game-setup-form-dial-text",
                LeftButton = "game-setup-form-dial-left-button",
                RightButton = "game-setup-form-dial-right-button"
            };

        private static readonly BannerComponent.Style s_BannerStyle =
            new()
            {
                Container = "game-setup-form-banner",
                Banner = "game-setup-form-banner-icon",
                Symbol = s_DialStyle,
                Pattern = s_DialStyle,
                PrimaryColor = s_DialStyle,
                SecondaryColor = s_DialStyle,
                SymbolColor = s_DialStyle
            };

        public BannerComponent Banner { get; }

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

            Banner = BannerComponent.Create(uiElementFactory, iconFactory, s_BannerStyle, bannerGenerator);
            Add(Banner);
        }
    }
}
