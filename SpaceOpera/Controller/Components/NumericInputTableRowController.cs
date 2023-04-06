using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputTableRowController<T> : IController, IFormElementController<T, int> where T : notnull
    {
        public EventHandler<ValueChangedEventArgs<T, int>>? ValueChanged { get; set; }

        public T Key { get; }

        private NumericInputTableRow<T>? _element;
        private NumericInputController<T>? _inputController;

        public NumericInputTableRowController(T key)
        {
            Key = key;
        }

        public void Bind(object @object)
        {
            _element = (NumericInputTableRow<T>)@object;
            _inputController = (NumericInputController<T>)_element.NumericInput.ComponentController;
        }

        public void Unbind()
        {
            _inputController = null;
            _element = null;
        }

        public int GetValue()
        {
            return _inputController!.GetValue();
        }

        public void SetValue(int value)
        {
            _inputController!.SetValue(value);
        }
    }
}
