using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui;
using SpaceOpera.View.Components.NumericInputs;
using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller.Components.NumericInputs
{
    public class ManualMultiCountInputController<T> : BaseMultiCountInputController<T> where T : notnull
    {
        public override IntInterval GetRange()
        {
            return IntInterval.Unbounded;
        }

        private SelectController<T>? _select;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var table = (ManualMultiCountInput<T>)_table!;
            table.AddButton.Controller.Clicked += HandleAdded;
            _select = (SelectController<T>)table.Select.ComponentController;
        }

        public override void Unbind()
        {
            _select = null;
            base.Unbind();
        }

        public override void SetValue(MultiCount<T>? value, bool notify = true)
        {
            value ??= new();
            ((ManualMultiCountInput<T>)_table!).SetRange(value.Keys);
            base.SetValue(value, notify);
        }

        protected override void BindElement(MultiCountInputRow<T> row)
        {
            base.BindElement(row);
            var controller = (ManualMultiCountInputRowController<T>)row.ComponentController;
            controller.Removed += HandleRemoved;
        }

        protected override void UnbindElement(MultiCountInputRow<T> row)
        {
            var controller = (ManualMultiCountInputRowController<T>)row.ComponentController;
            controller.Removed -= HandleRemoved;
            base.UnbindElement(row);
        }

        private void HandleAdded(object? sender, MouseButtonClickEventArgs e)
        {
            ((ManualMultiCountInput<T>)_table!).Add(_select!.GetValue()!);
            UpdateTotal();
        }

        private void HandleRemoved(object? sender, EventArgs e)
        {
            var controller = (IOptionController<T>)sender!;
            ((ManualMultiCountInput<T>)_table!).Remove(controller.Key);
            UpdateTotal();
        }
    }
}
