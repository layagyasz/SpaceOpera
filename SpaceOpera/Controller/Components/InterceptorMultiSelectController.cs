using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Game;
using SpaceOpera.View.Components;
using SpaceOpera.View.Game;

namespace SpaceOpera.Controller.Components
{
    public class InterceptorMultiSelectController<T> 
        : IController, IFormElementController<IEnumerable<T>> where T : notnull
    {
        public EventHandler<IEnumerable<T>?>? ValueChanged { get; set; }

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

        public void SetValue(IEnumerable<T>? value)
        {
            _element!.SetRange(value ?? Enumerable.Empty<T>());
        }

        private void HandleAdd(object? sender, T e)
        {
            _element!.Add(e);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.Action == ActionId.Unselect)
            {
                _element!.Remove((T)e.GetOnlyObject()!);
            }
        }
    }
}
