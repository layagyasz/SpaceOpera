using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View.Components;

namespace SpaceOpera.Controller.Components
{
    public class NumericInputTableRowController<T> : 
        IController, IOptionController<T>, IFormElementController<T, int> where T : notnull
    {
        public EventHandler<EventArgs>? Selected { get; set; }
        public EventHandler<ValueChangedEventArgs<T, int>>? ValueChanged { get; set; }

        public T Key { get; }

        private readonly NumericInputTable<T>.IConfiguration _configuration;

        private NumericInputTableRow<T>? _element;
        private ClassedUiElementController<ClassedUiElement>? _infoController;
        private NumericInputController<T>? _inputController;
        private int _defaultValue;

        public NumericInputTableRowController(T key, NumericInputTable<T>.IConfiguration configuration)
        {
            Key = key;
            _configuration = configuration;
        }

        public void Bind(object @object)
        {
            _element = (NumericInputTableRow<T>)@object;
            _element.Refreshed += HandleRefresh;
            _infoController = (ClassedUiElementController<ClassedUiElement>)_element.Info.Controller;
            _infoController.Clicked += HandleSelected;
            _inputController = (NumericInputController<T>)_element.NumericInput.ComponentController;
            _defaultValue = _configuration.GetValue(Key);
            _inputController.SetValue(_defaultValue);
            _inputController.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _inputController!.ValueChanged -= HandleValueChanged;
            _inputController = null;
            _infoController!.Clicked -= HandleSelected;
            _infoController = null;
            _element!.Refreshed -= HandleRefresh;
            _element = null;
        }

        public int GetValue()
        {
            return _inputController!.GetValue();
        }

        public void Reset()
        {
            _defaultValue = _configuration.GetValue(Key);
            _inputController!.SetValue(_defaultValue);
            _inputController!.SetRange(_configuration.GetRange(Key));
        }

        public void SetSelected(bool selected)
        {
            _infoController!.SetToggle(selected);
        }

        public void SetValue(int value)
        {
            _inputController!.SetValue(value);
        }

        private void HandleRefresh(object? sender, EventArgs e)
        {
            if (_inputController!.GetValue() == _defaultValue)
            {
                _defaultValue = _configuration.GetValue(Key);
                _inputController.SetValue(_defaultValue);
            }
            _inputController!.SetRange(_configuration.GetRange(Key));
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
