using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes;

namespace SpaceOpera.View.Panes.ResearchPanes
{
    public class ResearchPane : MultiTabGamePane
    {
        public enum TabId
        {
            Current
        }

        private static readonly string s_Title = "Research";

        private static readonly string s_ClassName = "pane-standard";
        private static readonly string s_TitleClassName = "pane-standard-title";
        private static readonly string s_CloseClass = "pane-standard-close";
        private static readonly string s_TabContainerClassName = "pane-tab-container";
        private static readonly string s_TabOptionClassName = "pane-tab-option";
        private static readonly string s_BodyClassName = "pane-body";

        public ResearchPane(
            IElementController controller, 
            Class @class,
            TextUiElement header, 
            IUiElement closeButton,
            UiCompoundComponent tabs, 
            IUiContainer body)
            : base(controller, @class, header, closeButton, tabs, body) { }

        public override void Populate(params object?[] args) { }

        public override void SetTab(object id) { }

        public static ResearchPane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new GamePaneController(),
                uiElementFactory.GetClass(s_ClassName),
                new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), s_Title), 
                uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    { 
                        new(TabId.Current, "Current")
                    },
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)),
                uiElementFactory.CreateTable(s_BodyClassName, Enumerable.Empty<IUiElement>()).Item1);
        }
    }
}
