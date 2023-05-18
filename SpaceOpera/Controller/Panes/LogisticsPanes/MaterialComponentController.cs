using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Panes.LogisticsPanes;

namespace SpaceOpera.Controller.Panes.LogisticsPanes
{
    public class MaterialComponentController : IController, IFormElementController<string, MultiCount<IMaterial>>
    {
        public EventHandler<ValueChangedEventArgs<string, MultiCount<IMaterial>?>>? ValueChanged { get; set; }

        public string Key { get; } 

        private MaterialComponent? _component;
        private SelectController<IMaterial>? _select;
        private SyncingNumericInputTableController<IMaterial>? _table;

        public MaterialComponentController(string key)
        {
            Key = key;
        }

        public void Bind(object @object)
        {
            _component = (MaterialComponent)@object!;
            _component.AddButton.Controller.Clicked += HandleMaterialAdded;
            _select = (SelectController<IMaterial>)_component.Select.Controller;
            _table = (SyncingNumericInputTableController<IMaterial>)_component.Table.ComponentController;
        }

        public void Unbind()
        {
            _table = null;
            _select = null;
            _component = null;
        }

        public MultiCount<IMaterial> GetValue()
        {
            return _table!.GetValues();
        }

        public void SetValue(MultiCount<IMaterial>? value)
        {
            value ??= new();
            _component!.SetRange(value.Keys);
            _table!.SetValue(value);
        }

        private void HandleMaterialAdded(object? sender, MouseButtonClickEventArgs e)
        {
            _component!.Add(_select!.GetValue()!);
        }
    }
}
