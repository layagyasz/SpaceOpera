using Cardamom.Ui;
using SpaceOpera.View.Components.NumericInputs;

namespace SpaceOpera.Controller.Components.NumericInputs
{
    public class ManualMultiCountInputRowController<T> : BaseMultiCountInputRowController<T> where T : notnull
    {
        public EventHandler<EventArgs>? Removed { get; set; }

        public ManualMultiCountInputRowController(T key)
            : base(key) { }

        public override int GetDefaultValue()
        {
            return 0;
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var row = (ManualMultiCountRow<T>)_element!;
            row.RemoveButton.Controller.Clicked += HandleRemoved;
        }

        public override void Unbind()
        {
            var row = (ManualMultiCountRow<T>)_element!;
            row.RemoveButton.Controller.Clicked -= HandleRemoved;
            base.Unbind();
        }

        private void HandleRemoved(object? sender, MouseButtonClickEventArgs e)
        {
            Removed?.Invoke(this, EventArgs.Empty);
        }
    }
}
