using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputController<T> : IController, IFormElementController<T, int>
    {
        public EventHandler<ValueChangedEventArgs<T, int>>? ValueChanged { get; set; }

        public T Key { get; }

        private NumericInput? _element;
        private int _value;

        public NumericInputController(T key)
        {
            Key = key;
        }

        public void Bind(object @object)
        {
            _element = (NumericInput)@object;
            _element.SubtractButton.Controller.Clicked += HandleSubtracted;
            _element.AddButton.Controller.Clicked += HandleAdded;
        }

        public void Unbind()
        {
            _element!.SubtractButton.Controller.Clicked -= HandleSubtracted;
            _element!.AddButton.Controller.Clicked -= HandleAdded;
            _element = null;
        }

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value)
        {
            _value = value;
            _element!.Text.SetText(_value.ToString());
            ValueChanged?.Invoke(this, new(Key, value));
        }

        private void HandleAdded(object? sender, MouseButtonClickEventArgs e)
        {
            SetValue(_value + GetDelta(e.Modifiers));
        }

        private void HandleSubtracted(object? sender, MouseButtonClickEventArgs e)
        {
            SetValue(_value - GetDelta(e.Modifiers));
        }

        private int GetDelta(KeyModifiers modifiers)
        {
            bool hasCtrl = modifiers.HasFlag(KeyModifiers.Control);
            bool hasShift = modifiers.HasFlag(KeyModifiers.Shift);
            if (hasCtrl && hasShift)
            {
                return 100;
            }
            if (hasCtrl)
            {
                return 50;
            }
            if (hasShift)
            {
                return 10;
            }
            return 1;
        }
    }
}
