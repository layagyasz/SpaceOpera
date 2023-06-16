using SpaceOpera.View.Game.Panes.LogisticsPanes;

namespace SpaceOpera.Controller.Game.Panes.LogisticsPanes
{
    public class LogisticPaneController : MultiTabGamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (LogisticsPane)_pane!;
            var controller = (IActionController)pane.Routes.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var pane = (LogisticsPane)_pane!;
            var controller = (IActionController)pane.Routes.ComponentController;
            controller.Interacted -= HandleInteraction;
            base.Unbind();
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e) 
        {
            Interacted?.Invoke(this, e);
        }
    }
}
