using Cardamom.Mathematics;
using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Mathematics;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class SliderInputController : IController, IFormElementController<int>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly IntInterval _range;

        private SliderInput? _component;
        private IElementController? _knobController;

        private int _value;

        public SliderInputController(IntInterval range)
        {
            _range = range;
        }

        public void Bind(object @object)
        {
            _component = (SliderInput)@object;
            _knobController = _component.Knob.Controller;
            _knobController.MouseDragged += HandleKnobDragged;
            SetValue(_value, /* notify= */ true);
        }

        public void Unbind()
        {
            _knobController!.MouseDragged -= HandleKnobDragged;
            _knobController = null;
            _component = null;
        }

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value, bool notify)
        {
            _value = _range.Clamp(value);
            if (_component != null)
            {
                _component.Knob.Position = new Vector3(GetPositionForValue(_value), 0, 0);
            }
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private float GetPositionForValue(int value)
        {
            float p = 1f * (value - _range.Minimum) / (_range.Maximum - _range.Minimum);
            var root = (ClassedUiElement)_component!.GetRoot();
            return p * (root.InternalSize.X - _component.Knob.Size.X);
        }

        private int GetValueForPosition(float position)
        {
            var root = (ClassedUiElement)_component!.GetRoot();
            float p = position / (root.InternalSize.X - _component.Knob.Size.X);
            return _range.Minimum + (int)(p * (_range.Maximum - _range.Minimum));
        }

        private void HandleKnobDragged(object? sender, MouseButtonDragEventArgs e)
        {
            var root = (ClassedUiElement)_component!.GetRoot();
            var mx = root.InternalSize.X - _component.Knob.Size.X;
            var p = MathHelper.Clamp(_component!.Knob.Position.X + e.Delta.X, 0, mx);
            _component!.Knob.Position = new(p, 0, 0);
            _value = GetValueForPosition(p);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
