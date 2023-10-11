using Cardamom.Ui.Controller;
using Cardamom.Ui;

namespace SpaceOpera.Controller.Components
{
    public class AdderComponentController<T> : DynamicComponentControllerBase, IAdderController<T>
    {
        public EventHandler<T>? Added { get; set; }

        protected override void BindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                var controller = (IAdderController<T>)row.ComponentController;
                controller!.Added += HandleAdded;
            }
        }

        protected override void UnbindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                var controller = (IAdderController<T>)row.ComponentController;
                controller!.Added -= HandleAdded;
            }
        }

        private void HandleAdded(object? sender, T element)
        {
            Added?.Invoke(this, element);
        }
    }
}
