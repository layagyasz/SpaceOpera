﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public abstract class BaseNumericInputTableRowController<T> :
        IController, IOptionController<T>, IFormElementController<T, int> where T : notnull
    {
        public EventHandler<EventArgs>? Selected { get; set; }
        public EventHandler<ValueChangedEventArgs<T, int>>? ValueChanged { get; set; }

        public T Key { get; }

        protected NumericInputTableRow<T>? _element;
        protected ClassedUiElementController<ClassedUiElement>? _infoController;
        protected NumericInputController<T>? _inputController;

        public BaseNumericInputTableRowController(T key)
        {
            Key = key;
        }

        public abstract int GetDefaultValue();

        public virtual void Bind(object @object)
        {
            _element = (NumericInputTableRow<T>)@object;
            _infoController = (ClassedUiElementController<ClassedUiElement>)_element.Info.Controller;
            _infoController.Clicked += HandleSelected;
            _inputController = (NumericInputController<T>)_element.NumericInput.ComponentController;
            _inputController.SetValue(GetDefaultValue());
            _inputController.ValueChanged += HandleValueChanged;
        }

        public virtual void Unbind()
        {
            _inputController!.ValueChanged -= HandleValueChanged;
            _inputController = null;
            _infoController!.Clicked -= HandleSelected;
            _infoController = null;
            _element = null;
        }

        public int GetValue()
        {
            return _inputController!.GetValue();
        }

        public void SetSelected(bool selected)
        {
            _infoController!.SetToggle(selected);
        }

        public void SetValue(int value)
        {
            _inputController!.SetValue(value);
        }

        private void HandleSelected(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                Selected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleValueChanged(object? sender, ValueChangedEventArgs<T, int> e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}