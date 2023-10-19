using Cardamom.Ui.Controller;
using SpaceOpera.View.Forms;

namespace SpaceOpera.Controller.Forms
{
    public class GenericFormController : IFormFieldController<FormValue>
    {
        public EventHandler<EventArgs>? Canceled { get; set; }
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly Dictionary<string, object> _hiddens;

        private Form? _form;

        public GenericFormController(Dictionary<string, object> hiddens)
        {
            _hiddens = hiddens;
        }

        public void Bind(object @object)
        {
            _form = (Form)@object!;
            foreach (var field in _form.Fields)
            {
                var controller = (IGenericFormFieldController)field.Value.ComponentController;
                controller.ValueChanged += HandleValueChanged;
            }
        }

        public void Unbind()
        {
            foreach (var field in _form!.Fields)
            {
                var controller = (IGenericFormFieldController)field.Value.ComponentController;
                controller.ValueChanged -= HandleValueChanged;
            }
            _form = null;
        }

        public FormValue GetValue()
        {
            var result = new FormValue();
            foreach (var field in _form!.Fields)
            {
                var controller = (IGenericFormFieldController)field.Value.ComponentController;
                result.Add(field.Key, controller.Get());
            }
            foreach (var hidden in _hiddens)
            {
                result.Add(hidden.Key, hidden.Value);
            }
            return result;
        }

        public void SetValue(FormValue? value, bool notify = true)
        {
            if (value != null)
            {
                foreach (var field in _form!.Fields)
                {
                    if (value.TryGetValue(field.Key, out var v))
                    {
                        var controller = (IGenericFormFieldController)field.Value.ComponentController;
                        controller.Set(v, /* notify= */ false);
                    }
                }
            }
            else
            {
                foreach (var field in _form!.Fields)
                {
                    var controller = (IGenericFormFieldController)field.Value.ComponentController;
                    controller.Set(null, /* notify= */ false);
                }
            }
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Cancel()
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
        }
    }
}
