﻿using Cardamom.Ui.Controller;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class InputWithTextController<T> : IController, IRandomizableFormFieldController<T>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly string _format;

        private InputWithText? _component;
        private IRandomizableFormFieldController<T>? _child;

        public InputWithTextController(string format)
        {
            _format = format;
        }

        public void Bind(object @object)
        {
            _component = (InputWithText)@object;
            _child = (IRandomizableFormFieldController<T>)_component.Input.ComponentController;
            _child.ValueChanged += HandleValueChanged;
            UpdateText();
        }

        public void Unbind()
        {
            _component = null;
            _child!.ValueChanged -= HandleValueChanged;
            _child = null;
        }

        public T? GetValue()
        {
            return _child!.GetValue();
        }

        public void Randomize(Random random, bool notify = true)
        {
            _child!.Randomize(random, /* notify= */ false);
            UpdateText();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetValue(T? value, bool notify)
        {
            _child!.SetValue(value, /* notify= */ false);
            UpdateText();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            UpdateText();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateText()
        {
            _component!.Text.SetText(string.Format(_format, GetValue()));
        }
    }
}
