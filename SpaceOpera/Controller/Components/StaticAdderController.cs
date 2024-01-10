using Cardamom.Ui;

namespace SpaceOpera.Controller.Components
{
    public class StaticAdderController<T> : IAdderController<T>
    {
        public EventHandler<T>? Added { get; set; }

        private IUiComponent? _component;
        private readonly T _value;

        public StaticAdderController(T value)
        {
            _value = value;
        }

        public void Bind(object @object)
        {
            _component = (IUiComponent)@object!;
            _component.Controller.Clicked += HandleClick;
        }

        public void Unbind()
        {
            _component!.Controller.Clicked -= HandleClick;
            _component = null;
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            Added?.Invoke(this, _value);
        }
    }
}
