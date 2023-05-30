using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class DynamicRadioController<T> : RadioController<T>
    {
        private readonly Func<T?> _valueFn;

        public DynamicRadioController(Func<T?> valueFn)
            : base(valueFn())
        {
            _valueFn = valueFn;
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var component = (DynamicUiCompoundComponent)_component!;
            component.Refreshed += HandleRefresh;
        }

        public override void Unbind()
        {
            var component = (DynamicUiCompoundComponent)_component!;
            component.Refreshed -= HandleRefresh;
            base.Unbind();
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            SetValue(_valueFn());
        }
    }
}
