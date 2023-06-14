using SpaceOpera.View.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Panes.DiplomacyPanes
{
    public class DiplomacyPaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (DiplomacyPane)_pane!;
            var controller = (IActionController)pane.Relations.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var pane = (DiplomacyPane)_pane!;
            var controller = (IActionController)pane.Relations.ComponentController;
            controller.Interacted -= HandleInteraction;
            base.Unbind();
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
