using Cardamom.Ui;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class ManualNumericInputTableRowController<T> : BaseNumericInputTableRowController<T> where T : notnull
    {
        public EventHandler<EventArgs>? Removed { get; set; }

        public ManualNumericInputTableRowController(T key)
            : base(key) { }

        public override int GetDefaultValue()
        {
            return 0;
        }

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var row = (ManualNumericInputTableRow<T>)_element!;
            row.RemoveButton.Controller.Clicked += HandleRemoved;
        }

        public override void Unbind()
        {
            var row = (ManualNumericInputTableRow<T>)_element!;
            row.RemoveButton.Controller.Clicked -= HandleRemoved;
            base.Unbind();
        }

        private void HandleRemoved(object? sender, MouseButtonClickEventArgs e)
        {
            Removed?.Invoke(this, EventArgs.Empty);
        }
    }
}
