using Cardamom.Mathematics;
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
        private IntInterval _range;

        public NumericInputController(T key, IntInterval range)
        {
            Key = key;
            _range = range;
        }

        public void Bind(object @object)
        {
            _element = (NumericInput)@object;
            _element.SubtractButton.Controller.Clicked += HandleSubtracted;
            _element.AddButton.Controller.Clicked += HandleAdded;
            UpdateString();
        }

        public void Unbind()
        {
            _element!.SubtractButton.Controller.Clicked -= HandleSubtracted;
            _element!.AddButton.Controller.Clicked -= HandleAdded;
            _element = null;
        }

        public void SetRange(IntInterval range)
        {
            _range = range;
            SetValue(_value);
            UpdateString();
        }

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value)
        {
            int newValue = _range.Clamp(value);
            if (_value != newValue)
            {
                _value = newValue;
                UpdateString();
                ValueChanged?.Invoke(this, new(Key, value));
            }
        }

        private void HandleAdded(object? sender, MouseButtonClickEventArgs e)
        {
            SetValue(_value + GetDelta(e.Modifiers));
        }

        private void HandleSubtracted(object? sender, MouseButtonClickEventArgs e)
        {
            SetValue(_value - GetDelta(e.Modifiers));
        }

        private void UpdateString()
        {
            _element!.Text.SetText(ToDisplayedString(_value, _range));
        }

        private static int GetDelta(KeyModifiers modifiers)
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

        private static string ToDisplayedString(int value, IntInterval range)
        {
            return range.Maximum < int.MaxValue ? $"{value}/{range.Maximum}" : value.ToString();
        }
    }
}
