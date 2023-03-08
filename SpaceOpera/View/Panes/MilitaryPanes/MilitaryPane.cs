using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes;

namespace SpaceOpera.View.Panes.MilitaryPanes
{
    public class MilitaryPane : GamePane
    {
        public enum TabId
        {
            Army,
            Fleet
        }

        private static readonly string s_Title = "Military";

        private static readonly string s_ClassName = "pane-standard";
        private static readonly string s_TitleClassName = "pane-standard-title";
        private static readonly string s_CloseClass = "pane-standard-close";
        private static readonly string s_TabContainerClassName = "pane-tab-container";
        private static readonly string s_TabOptionClassName = "pane-tab-option";
        private static readonly string s_BodyClassName = "pane-body";

        public MilitaryPane(
            IElementController controller,
            Class @class,
            IUiElement header,
            IUiElement closeButton,
            UiCompoundComponent tabs, 
            IUiContainer body)
            : base(controller, @class, header, closeButton, tabs, body) { }

        public override void Populate(params object?[] args) { }

        public override void SetTab(object id) { }

        public static MilitaryPane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new GamePaneController(),
                uiElementFactory.GetClass(s_ClassName),
                uiElementFactory.CreateTextButton(s_TitleClassName, s_Title).Item1, 
                uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>() 
                    { 
                        new(TabId.Army, "Army"),
                        new(TabId.Fleet, "Fleet")
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)),
                uiElementFactory.CreateTable(s_BodyClassName, Enumerable.Empty<IUiElement>()).Item1);
        }
    }
}
