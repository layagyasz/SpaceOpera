using SpaceOpera.Controller.Components;
using SpaceOpera.View.Panes.DesignPanes;

namespace SpaceOpera.Controller.Panes.DesignPanes
{
    public class DesignPaneController : MultiTabGamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = _pane as DesignPane;
            var tableController = pane!.DesignTable.Controller as IActionController;
            tableController!.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var pane = _pane as DesignPane;
            var tableController = pane!.DesignTable.Controller as IActionController;
            tableController!.Interacted -= HandleInteraction;
            base.Unbind();
        }

        private void HandleInteraction(object? @object, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
