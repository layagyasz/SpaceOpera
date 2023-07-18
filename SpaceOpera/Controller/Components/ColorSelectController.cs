using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class ColorSelectController : IController, IRandomizableFormFieldController<Color4>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private ColorSelect? _component;
        private IRandomizableFormFieldController<Color4>? _options;

        private bool _focus;

        public void Bind(object @object)
        {
            _component = (ColorSelect)@object;
            _component.Root.Controller.Focused += HandleFocused;
            _component.Root.Controller.FocusLeft += HandleFocusLeft;
            _component.Root.Controller.MouseEntered += HandleMouseEntered;
            _component.Root.Controller.MouseLeft += HandleMouseLeft;
            _options = (IRandomizableFormFieldController<Color4>)_component.Options.ComponentController;
            _options.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component!.Root.Controller.Focused -= HandleFocused;
            _component!.Root.Controller.FocusLeft -= HandleFocusLeft;
            _component!.Root.Controller.MouseEntered -= HandleMouseEntered;
            _component!.Root.Controller.MouseLeft -= HandleMouseLeft;
            _component = null;
            _options!.ValueChanged -= HandleValueChanged;
            _options = null;
        }

        public Color4 GetValue()
        {
            return _options!.GetValue();
        }

        public void Randomize(Random random, bool notify = true)
        {
            _options!.Randomize(random, notify);
        }

        public void SetValue(Color4 value, bool notify = true)
        {
            _component!.Root.SetColor(value);
            _options!.SetValue(value, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleFocused(object? sender, EventArgs e)
        {
            _component!.SetOpen(true);
            _focus = true;
        }

        private void HandleFocusLeft(object? sender, EventArgs e)
        {
            _component!.SetOpen(false);
            _focus = false;
        }

        private void HandleMouseEntered(object? sender, EventArgs e)
        {
            _component!.SetOpen(_focus);
        }

        private void HandleMouseLeft(object? sender, EventArgs e)
        {
            _component!.SetOpen(false);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            _component!.Root.SetColor(_options!.GetValue());
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
