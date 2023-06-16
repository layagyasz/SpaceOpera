using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game;

namespace SpaceOpera.Controller.Components
{
    public class ActionComponentController : DynamicComponentControllerBase, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public override void BindElement(IUiElement element)
        {
            if (element is UiCompoundComponent row)
            {
                var controller = (IActionController)row.ComponentController;
                controller!.Interacted += HandleInteraction;
            }
        }

        public override void UnbindElement(IUiElement element)
        {
            if (element is UiCompoundComponent row)
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
