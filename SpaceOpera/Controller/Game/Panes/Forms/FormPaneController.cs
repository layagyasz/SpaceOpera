using Cardamom.Ui;
using Cardamom.Utils.Suppliers;
using SpaceOpera.Controller.Forms;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Game.Panes.Forms;

namespace SpaceOpera.Controller.Game.Panes.Forms
{
    public class FormPaneController : GamePaneController
    {
        private GenericFormController? _form;
        private Promise<FormValue>? _promise;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane!.Populated += HandlePopulate;
            ((FormPane)_pane).Submit.Controller.Clicked += HandleSubmit;
        }

        public override void Unbind()
        {
            _pane!.Populated -= HandlePopulate;
            ((FormPane)_pane).Submit.Controller.Clicked -= HandleSubmit;
            base.Unbind();
        }

        public override void Close()
        {
            base.Close();
            _promise?.Cancel();
            _promise = null;
        }

        private void HandlePopulate(object? sender, EventArgs e)
        {
            _form = (GenericFormController)((FormPane)_pane!).GetForm().ComponentController;
            _promise = ((FormPane)_pane!).GetPromise();
        }

        private void HandleSubmit(object? sender, MouseButtonClickEventArgs e)
        {
            _promise!.Set(_form!.GetValue());
            Close();
        }
    }
}
