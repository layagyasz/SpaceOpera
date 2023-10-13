using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.Forms;
using SpaceOpera.View.Forms;

namespace SpaceOpera.View.Game.Panes.Forms
{
    public class FormPane : SimpleGamePane
    {
        private static readonly string s_Container = "form-pane";
        private static readonly string s_Title = "form-pane-title";
        private static readonly string s_Close = "form-pane-close";

        private IUiComponent? _form;

        public FormPane(UiElementFactory uiElementFactory)
            : base(
                  new FormPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        { }

        public override void Populate(params object?[] args)
        {
            _form = (Form)args[0]!;
            SetBody(_form);
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
