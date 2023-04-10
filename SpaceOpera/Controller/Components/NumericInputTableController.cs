using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputTableController<T> : IController where T : notnull
    {
        public EventHandler<ValueEventArgs<T?>>? RowSelected { get; set; }
        public EventHandler<EventArgs>? Submitted { get; set; }

        private readonly NumericInputTable<T>.IConfiguration _configuration;

        private NumericInputTable<T>? _table;
        private RadioController<T>? _tableController;

        public NumericInputTableController(NumericInputTable<T>.IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Bind(object @object)
        {
            _table = (NumericInputTable<T>)@object;
            _table.Refreshed += HandleRefresh;
            _table.Table.ElementAdded += HandleElementAdded;
            _table.Table.ElementRemoved += HandleElementRemoved;
            _table.Submit.Controller.Clicked += HandleSubmit;
            _tableController = (RadioController<T>)_table.Table.ComponentController;
            _tableController.ValueChanged += HandleRowSelected;
        }

        public void Unbind()
        {
            _tableController!.ValueChanged -= HandleRowSelected;
            _tableController = null;
            _table!.Refreshed -= HandleRefresh;
            _table!.Table.ElementAdded -= HandleElementAdded;
            _table!.Table.ElementRemoved -= HandleElementRemoved;
            _table!.Submit.Controller.Clicked -= HandleSubmit;
            _table = null;
        }

        public MultiCount<T> GetDeltas()
        {
            return _table!.Table
                .Select(x => ((UiCompoundComponent)x).ComponentController)
                .Cast<NumericInputTableRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetValue() - _configuration.GetValue(x.Key)))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public MultiCount<T> GetValues()
        {
            return _table!.Table
                .Select(x => ((UiCompoundComponent)x).ComponentController)
                .Cast<NumericInputTableRowController<T>>()
                .Select(x => new KeyValuePair<T, int>(x.Key, x.GetValue()))
                .Where(x => x.Value != 0)
                .ToMultiCount(x => x.Key, x => x.Value);
        }

        public void Reset()
        {
            _table!.Refresh();
            ((TableController)_table!.Table.Controller).ResetOffset();
            foreach (var row in _table!.Table.Cast<NumericInputTableRow<T>>())
            {
                ((NumericInputTableRowController<T>)row.ComponentController).Reset();
            }
            UpdateTotal();
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

        private void HandleRefresh(object? @object, EventArgs e)
        {
            UpdateTotal();
        }

        private void HandleRowSelected(object? sender, ValueChangedEventArgs<string, T?> e)
        {
            RowSelected?.Invoke(this, new(e.Value));
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
            _table!.Total.SetText(ToDisplayedString(total, _configuration.GetRange()));
        }

        private static string ToDisplayedString(int value, IntInterval range)
        {
            return range.Maximum < int.MaxValue ? $"{value}/{range.Maximum}" : value.ToString();
        }
    }
}
