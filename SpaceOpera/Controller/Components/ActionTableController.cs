using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;

namespace SpaceOpera.Controller.Components
{
    public class ActionTableController : TableController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public override void BindElement(IUiElement element)
        {
            base.BindElement(element);
            var controller = element.Controller as IActionController;
            controller!.Interacted += HandleInteraction;
        }

        public override void UnbindElement(IUiElement element)
        {
            base.UnbindElement(element);
            var controller = element.Controller as IActionController;
            controller!.Interacted -= HandleInteraction;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
