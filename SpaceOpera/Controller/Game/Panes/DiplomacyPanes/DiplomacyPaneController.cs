using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomacyPaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (DiplomaticRelationPane)_pane!;
            var controller = (IActionController)pane.Relations.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var pane = (DiplomaticRelationPane)_pane!;
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
