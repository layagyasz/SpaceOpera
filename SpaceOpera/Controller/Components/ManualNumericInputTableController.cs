using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using SpaceOpera.View.Components;
using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Components
{
    public class ManualNumericInputTableController<T> : BaseNumericInputTableController<T> where T : notnull
    {
        public override IntInterval GetRange()
        {
            return IntInterval.Unbounded;
        }

        private SelectController<T>? _select;

        public ManualNumericInputTableController(string key)
            : base(key) { }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var table = (ManualNumericInputTable<T>)_table!;
            table.AddButton.Controller.Clicked += HandleAdded;
            _select = (SelectController<T>)table.Select.Controller;
        }

        public override void Unbind()
        {
            _select = null;
            base.Unbind();
        }

        public override void SetValue(MultiCount<T>? value)
        {
            value ??= new();
            ((ManualNumericInputTable<T>)_table!).SetRange(value.Keys);
            base.SetValue(value);
        }

        protected override void BindElement(NumericInputTableRow<T> row)
        {
            base.BindElement(row);
            var controller = (ManualNumericInputTableRowController<T>)row.ComponentController;
            controller.Removed += HandleRemoved;
        }

        protected override void UnbindElement(NumericInputTableRow<T> row)
        {
            var controller = (ManualNumericInputTableRowController<T>)row.ComponentController;
            controller.Removed -= HandleRemoved;
            base.UnbindElement(row);
        }

        private void HandleAdded(object? sender, MouseButtonClickEventArgs e)
        {
            ((ManualNumericInputTable<T>)_table!).Add(_select!.GetValue()!);
            UpdateTotal();
        }

        private void HandleRemoved(object? sender, EventArgs e)
        {
            var controller = (IOptionController<T>)sender!;
            ((ManualNumericInputTable<T>)_table!).Remove(controller.Key);
            UpdateTotal();
        }
    }
}
