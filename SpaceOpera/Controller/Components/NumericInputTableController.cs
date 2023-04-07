using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputTableController<T> : IController where T : notnull
    {
        public EventHandler<EventArgs>? Submitted { get; set; }

        private NumericInputTable<T>? _table;

        public void Bind(object @object)
        {
            _table = (NumericInputTable<T>)@object;
            _table.Table.ElementAdded += HandleElementAdded;
            _table.Table.ElementRemoved += HandleElementRemoved;
            _table.Submit.Controller.Clicked += HandleSubmit;
            UpdateTotal();
        }

        public void Unbind()
        {
            _table!.Table.ElementAdded -= HandleElementAdded;
            _table!.Table.ElementRemoved -= HandleElementRemoved;
            _table!.Submit.Controller.Clicked -= HandleSubmit;
            _table = null;
        }

        private void BindElement(NumericInputTableRow<T> row)
        {
            var controller = (NumericInputTableRowController<T>)row.ComponentController;
            controller.ValueChanged += HandleValueChanged;
        }

        private void HandleElementAdded(object? @object, ElementEventArgs e)
        {
            BindElement((NumericInputTableRow<T>)e.Element);
        }

        private void HandleElementRemoved(object? @object, ElementEventArgs e)
        {
            UnbindElement((NumericInputTableRow<T>)e.Element);
        }

        private void HandleSubmit(object? @object, MouseButtonClickEventArgs e)
        {
            Submitted?.Invoke(this, EventArgs.Empty);
        }

        private void HandleValueChanged(object? @object, ValueChangedEventArgs<T, int> e)
        {
            UpdateTotal();
        }

        private void UnbindElement(NumericInputTableRow<T> row)
        {
            var controller = (NumericInputTableRowController<T>)row.ComponentController;
            controller.ValueChanged -= HandleValueChanged;
        }

        private void UpdateTotal()
        {
            int total = 0;
            foreach (var row in _table!.Table.Cast<NumericInputTableRow<T>>())
            {
                var controller = (NumericInputTableRowController<T>)row.ComponentController;
                total += controller.GetValue();
            }
            _table!.Total.SetText(total.ToString());
        }
    }
}
