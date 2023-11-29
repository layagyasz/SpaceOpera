using Cardamom.Ui;
using Cardamom.Utils.Suppliers.Promises;
using SpaceOpera.Controller.Forms;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Game.Panes.Forms;

namespace SpaceOpera.Controller.Game.Panes.Forms
{
    public class FormPaneController : GamePaneController
    {
        private GenericFormController? _form;
        private RemotePromise<FormValue>? _promise;

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
            _form = null;
            _promise?.Cancel();
            _promise = null;
        }

        public void Submit()
        {
            _form!.ValueChanged -= HandleValueChanged;
            _promise!.Set(_form!.GetValue());
            _form = null;
            _promise = null;
            Close();
        }

        private void HandlePopulate(object? sender, EventArgs e)
        {
            _promise?.Cancel();
            _form = (GenericFormController)((FormPane)_pane!).GetForm().ComponentController;
            _form.ValueChanged += HandleValueChanged;
            _promise = ((FormPane)_pane!).GetPromise();
        }

        private void HandleSubmit(object? sender, MouseButtonClickEventArgs e)
        {
            Submit();
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            if (((FormPane)_pane!).GetForm().AutoSubmit)
            {
                Submit();
            }
        }
    }
}
