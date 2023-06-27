using Cardamom;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class DialSelectController<T> : IController, IRandomizableFormFieldController<T>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly List<SelectOption<T>> _range;
        private readonly bool _wrap;

        private int _valueIndex;

        private DialSelect? _component;

        public DialSelectController(IEnumerable<SelectOption<T>> range, T initialValue, bool wrap=false)
        {
            _range = range.ToList();
            _wrap = wrap;

            _valueIndex = _range.FindIndex(x => Equals(x.Value, initialValue));
        }

        public void Bind(object @object)
        {
            _component = (DialSelect)@object;
            _component.LeftButton.Controller.Clicked += HandleLeft;
            _component.RightButton.Controller.Clicked += HandleRight;
            UpdateText();
        }

        public void Unbind()
        {
            _component!.LeftButton.Controller.Clicked -= HandleLeft;
            _component!.RightButton.Controller.Clicked -= HandleRight;
            _component = null;
        }

        public T? GetValue()
        {
            return _range[_valueIndex].Value;
        }

        public void Randomize(Random random,  bool notify = true)
        {
            SetValueIndex(random.Next(_range.Count), notify);
        }

        public void SetValue(T? value, bool notify = true)
        {
            if (Equals(_range[_valueIndex].Value, value))
            {
                return;
            }
            SetValueIndex(_range.FindIndex(x => Equals(x.Value, value)), notify);
        }

        private void HandleLeft(object? sender, MouseButtonClickEventArgs e)
        {
            Rotate(-1);
        }

        private void HandleRight(object? sender, MouseButtonClickEventArgs e)
        {
            Rotate(1);
        }

        private void Rotate(int delta)
        {
            if (_wrap)
            {
                SetValueIndex((_valueIndex + delta + _range.Count) % _range.Count, /* notify= */ true);
            }
            else
            {
                SetValueIndex(Math.Max(0, Math.Min(_valueIndex + delta, _range.Count - 1)), /* notify= */ true);
            }
        }

        private void SetValueIndex(int index, bool notify)
        {
            Precondition.Check(index >= 0 && index < _range.Count);
            _valueIndex = index;
            UpdateText();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateText()
        {
            _component!.Text.SetText(_range[_valueIndex].Text);
        }
    }
}
