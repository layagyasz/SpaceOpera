using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.MilitaryPanes;

namespace SpaceOpera.Controller.Game.Panes.MilitaryPanes
{
    public class MilitaryPaneController : MultiTabGamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (MilitaryPane)_pane!;
            BindTab((ITabController)pane.RecruitmentTab.ComponentController);
        }

        public override void Unbind()
        {
            var pane = (MilitaryPane)_pane!;
            UnbindTab((ITabController)pane.RecruitmentTab.ComponentController);
            base.Unbind();
        }

        private void BindTab(ITabController controller)
        {
            controller.Interacted += HandleInteraction;
            controller.OrderCreated += HandleOrder;
        }

        private void UnbindTab(ITabController controller)
        {
            controller.Interacted -= HandleInteraction;
            controller.OrderCreated -= HandleOrder;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }

        private void HandleOrder(object? sender, IOrder e)
        {
            OrderCreated?.Invoke(this, e);
        }
    }
}
