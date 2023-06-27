using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class ActionTableController<T> : IController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<T?>? RowSelected { get; set; }

        private ActionTable<T>? _table;

        public void Bind(object @object)
        {
            _table = (ActionTable<T>)@object!;
            var headerController = (IActionController)_table.Header.ComponentController;
            headerController.Interacted += HandleInteraction;
            var tableController = (RadioController<T>)_table.Table.ComponentController;
            tableController.ValueChanged += HandleRowSelected;
            _table.Table.ElementAdded += HandleRowAdded;
            _table.Table.ElementRemoved += HandleRowRemoved;
            foreach (var row in _table.Table)
            {
                BindRow(row);
            }
        }

        public void Unbind() 
        {
            var headerController = (IActionController)_table!.Header.ComponentController;
            headerController.Interacted -= HandleInteraction;
            var tableController = (RadioController<T>)_table.Table.ComponentController;
            tableController.ValueChanged -= HandleRowSelected;
            _table.Table.ElementAdded -= HandleRowAdded;
            _table.Table.ElementRemoved -= HandleRowRemoved;
            foreach (var row in _table.Table)
            {
                UnbindRow(row);
            }
            _table = null;
        }

        private void BindRow(IUiElement row)
        {
            var component = (UiCompoundComponent)row;
            var controller = (IActionController)component.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        private void UnbindRow(IUiElement row)
        {
            var component = (UiCompoundComponent)row;
            var controller = (IActionController)component.ComponentController;
            controller.Interacted -= HandleInteraction;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }

        private void HandleRowAdded(object? sender, ElementEventArgs e)
        {
            BindRow((IUiElement)e.Element);
        }

        private void HandleRowRemoved(object? sender, ElementEventArgs e)
        {
            UnbindRow((IUiElement)e.Element);
        }

        private void HandleRowSelected(object? sender, EventArgs e)
        {
            RowSelected?.Invoke(this, ((IFormFieldController<T>)sender!).GetValue());
        }
    }
}