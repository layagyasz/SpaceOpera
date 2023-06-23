using Cardamom.Ui.Controller;
using SpaceOpera.View;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class InterceptorMultiSelectController<T> 
        : IController, IFormElementController<IEnumerable<T>> where T : notnull
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private InterceptorMultiSelect<T>? _element;

        public void Bind(object @object)
        {
            _element = (InterceptorMultiSelect<T>)@object;
            var tableController = (IActionController)_element.Table.ComponentController;
            tableController.Interacted += HandleInteraction;
            var adderController = (IAdderController<T>)_element.Adder.Controller;
            adderController.Added += HandleAdd;
        }

        public void Unbind()
        {
            var tableController = (IActionController)_element!.Table.ComponentController;
            tableController.Interacted -= HandleInteraction;
            _element = null;
        }

        public IEnumerable<T> GetValue()
        {
            return _element!.Table.Cast<IKeyedUiElement<T>>().Select(x => x.Key);
        }

        public void SetValue(IEnumerable<T>? value, bool notify = true)
        {
            _element!.SetRange(value ?? Enumerable.Empty<T>());
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleAdd(object? sender, T e)
        {
            _element!.Add(e);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.Action == ActionId.Unselect)
            {
                _element!.Remove((T)e.GetOnlyObject()!);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
