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

        private static readonly string s_Container = "pane-standard";
        private static readonly string s_Title = "pane-standard-title";
        private static readonly string s_Close = "pane-standard-close";
        private static readonly string s_TabContainer = "pane-tab-container";
        private static readonly string s_TabOption = "pane-tab-option";
        private static readonly string s_Body = "pane-body";

        public ResearchPane(
            IElementController controller, 
            Class @class,
            TextUiElement header, 
            IUiElement closeButton,
            UiCompoundComponent tabs)
            : base(controller, @class, header, closeButton, tabs) { }

        public override void Populate(params object?[] args)
        {
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id) { }

        public static ResearchPane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new GamePaneController(),
                uiElementFactory.GetClass(s_Container),
                new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Research"), 
                uiElementFactory.CreateSimpleButton(s_Close).Item1,
                TabBar<TabId>.Create(
                    new List<TabBar<TabId>.Definition>()
                    { 
                        new(TabId.Current, "Current")
                    },
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)));
        }
    }
}
