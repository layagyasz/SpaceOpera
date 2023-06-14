using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Panes.StellarBodyRegionPanes
{
    public class StellarBodyRegionPaneController : MultiTabGamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (StellarBodyRegionPane)_pane!;
            pane.Populated += HandlePopulated;
            BindTab((ITabController)pane.StructureTab.Controller);
            BindTab((ITabController)pane.ProjectTab.Controller);
        }

        public override void Unbind()
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.Populated -= HandlePopulated;
            UnbindTab((ITabController)pane.StructureTab.Controller);
            UnbindTab((ITabController)pane.ProjectTab.Controller);
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
        
        private void HandlePopulated(object? sender, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            ((AutoNumericInputTableController<Structure>)pane.StructureTab.Structures.ComponentController).Reset();
            ((AutoNumericInputTableController<Recipe>)pane.StructureTab.Recipes.ComponentController).Reset();
        }
    }
}
