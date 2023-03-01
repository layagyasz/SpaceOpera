using Cardamom.Ui;
using SpaceOpera.Controller.Panes;

namespace SpaceOpera.View.Panes.Military
{
    public class MilitaryPane
    {
        private static readonly string s_Title = "Military";

        private static readonly string s_ClassName = "pane-standard";
        private static readonly string s_TitleClassName = "pane-standard-title";
        private static readonly string s_CloseClass = "pane-standard-close";

        public static GamePane Create(UiElementFactory uiElementFactory)
        {
            return new(
                new GamePaneController(),
                uiElementFactory.GetClass(s_ClassName),
                uiElementFactory.CreateTextButton(s_TitleClassName, s_Title).Item1, 
                uiElementFactory.CreateTextButton(s_CloseClass, "X").Item1);
        }
    }
}
