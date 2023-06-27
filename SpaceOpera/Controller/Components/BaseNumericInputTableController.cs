using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public abstract class BaseNumericInputTableController<T> 
        : IController, IFormFieldController<MultiCount<T>> where T : notnull
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }
        public EventHandler<T?>? RowSelected { get; set; }

        public string Key { get; }

        protected BaseNumericInputTable<T>? _table;
        protected RadioController<T>? _tableController;

        public BaseNumericInputTableController(string key)
        {
            Key = key;
        }

        public abstract IntInterval GetRange();

        public virtual void Bind(object @object)
        {
            _table = (BaseNumericInputTable<T>)@object;
            _table.Table.ElementAdded += HandleElementAdded;
            _table.Table.ElementRemoved += HandleElementRemoved;
            _tableController = (RadioController<T>)_table.Table.ComponentController;
            _tableController.ValueChanged += HandleRowSelected;

            UpdateTotal();
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

        public MultiCount<T> GetValue()
        {
            return _table!.Table
                .Select(x => ((UiCompoundComponent)x).ComponentController)
                .Cast<BaseNumericInputTableRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetValue()))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public virtual void SetValue(MultiCount<T>? value, bool notify = true)
        {
            value ??= new();
            foreach (var entry in value)
            {
                if (_table!.TryGetRow(entry.Key, out var row))
                {
                    var controller = (BaseNumericInputTableRowController<T>)row!.ComponentController;
                    controller.SetValue(entry.Value, /* notify= */ false);
                }
            }
            UpdateTotal();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetValue(T key, int value, bool notify)
        {
            if (_table!.TryGetRow(key, out var row))
            {
                var controller = (BaseNumericInputTableRowController<T>)row!.ComponentController;
                controller.SetValue(value, /* notify= */ false);
            }
            UpdateTotal();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void BindElement(NumericInputTableRow<T> row)
        {
            var controller = (BaseNumericInputTableRowController<T>)row.ComponentController;
            controller.ValueChanged += HandleValueChanged;
        }

        protected virtual void UnbindElement(NumericInputTableRow<T> row)
        {
            var controller = (BaseNumericInputTableRowController<T>)row.ComponentController;
            controller.ValueChanged -= HandleValueChanged;
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

        private void HandleRowSelected(object? sender, EventArgs e)
        {
            RowSelected?.Invoke(this, ((IFormFieldController<T>)sender!).GetValue());
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            UpdateTotal();
            ValueChanged?.Invoke(this, EventArgs.Empty);
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
