using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public abstract class BaseNumericInputTableController<T> : IController where T : notnull
    {
        public EventHandler<ValueEventArgs<T?>>? RowSelected { get; set; }

        protected NumericInputTable<T>? _table;
        protected RadioController<T>? _tableController;

        public abstract IntInterval GetRange();

        public virtual void Bind(object @object)
        {
            _table = (NumericInputTable<T>)@object;
            _table.Table.ElementAdded += HandleElementAdded;
            _table.Table.ElementRemoved += HandleElementRemoved;
            _tableController = (RadioController<T>)_table.Table.ComponentController;
            _tableController.ValueChanged += HandleRowSelected;
        }

        public virtual void Unbind()
        {
            _tableController!.ValueChanged -= HandleRowSelected;
            _tableController = null;
            _table!.Table.ElementAdded -= HandleElementAdded;
            _table!.Table.ElementRemoved -= HandleElementRemoved;
            _table = null;
        }

        public T GetSelected()
        {
            return ((RadioController<T>)_table!.Table.ComponentController).GetValue()!;
        }

        public MultiCount<T> GetValues()
        {
            return _table!.Table
                .Select(x => ((UiCompoundComponent)x).ComponentController)
                .Cast<BaseNumericInputTableRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetValue()))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        private void BindElement(NumericInputTableRow<T> row)
        {
            var controller = (BaseNumericInputTableRowController<T>)row.ComponentController;
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

        private void HandleRefresh(object? @object, EventArgs e)
        {
            UpdateTotal();
        }

        private void HandleRowSelected(object? sender, ValueChangedEventArgs<string, T?> e)
        {
            RowSelected?.Invoke(this, new(e.Value));
        }

        private void HandleValueChanged(object? @object, ValueChangedEventArgs<T, int> e)
        {
            UpdateTotal();
        }

        private void UnbindElement(NumericInputTableRow<T> row)
        {
            var controller = (BaseNumericInputTableRowController<T>)row.ComponentController;
            controller.ValueChanged -= HandleValueChanged;
        }

        protected void UpdateTotal()
        {
            int total = 0;
            foreach (var row in _table!.Table.Cast<NumericInputTableRow<T>>())
            {
                var controller = (BaseNumericInputTableRowController<T>)row.ComponentController;
                total += controller.GetValue();
            }
            _table!.Total.SetText(ToDisplayedString(total, GetRange()));
        }

        private static string ToDisplayedString(int value, IntInterval range)
        {
            return range.Maximum < int.MaxValue ? $"{value}/{range.Maximum}" : value.ToString();
        }
    }
}
