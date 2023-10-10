using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace SpaceOpera.Controller.Components
{
    public class ActionComponentController : DynamicComponentControllerBase, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        protected override void BindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                var controller = (IActionController)row.ComponentController;
                controller!.Interacted += HandleInteraction;
            }
        }

        protected override void UnbindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                var controller = (IActionController)row.ComponentController;
                controller!.Interacted -= HandleInteraction;
            }
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
