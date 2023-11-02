using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.View.Components.NumericInputs;

namespace SpaceOpera.Controller.Components.NumericInputs
{
    public abstract class BaseMultiCountInputRowController<T> :
        IController, IOptionController<T>, IFormFieldController<int> where T : notnull
    {
        public EventHandler<EventArgs>? Selected { get; set; }
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        public T Key { get; }

        protected MultiCountInputRow<T>? _element;
        protected ClassedUiElementController<ClassedUiElement>? _infoController;
        protected NumericInputController? _inputController;

        public BaseMultiCountInputRowController(T key)
        {
            Key = key;
        }

        public abstract int GetDefaultValue();

        public virtual void Bind(object @object)
        {
            _element = (MultiCountInputRow<T>)@object;
            _infoController = (ClassedUiElementController<ClassedUiElement>)_element.Info.Controller;
            _infoController.Clicked += HandleSelected;
            _inputController = (NumericInputController)_element.NumericInput.ComponentController;
            _inputController.SetValue(GetDefaultValue(), /* notify= */ false);
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

        public void SetValue(int value, bool notify = true)
        {
            _inputController!.SetValue(value, notify);
        }

        private void HandleSelected(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                Selected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
