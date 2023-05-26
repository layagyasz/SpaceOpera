using Cardamom.Mathematics;
using Cardamom.Trackers;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using SpaceOpera.View.Components;

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
            table.AddButton.Controller.Clicked += HandleMaterialAdded;
            _select = (SelectController<T>)table.Select.Controller;
        }

        public override void Unbind()
        {
            _select = null;
            base.Unbind();
        }

        public override void SetValue(MultiCount<T>? value)
        {
            base.SetValue(value);
            value ??= new();
            ((ManualNumericInputTable<T>)_table!).SetRange(value.Keys);
        }

        private void HandleMaterialAdded(object? sender, MouseButtonClickEventArgs e)
        {
            ((ManualNumericInputTable<T>)_table!).Add(_select!.GetValue()!);
        }
    }
}
