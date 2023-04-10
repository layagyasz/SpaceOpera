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
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            structureTableController.RowSelected += HandleStructureSelected;
            structureTableController.Submitted += HandleStructureSubmitted;
        }

        public override void Unbind()
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.Populated -= HandlePopulated;
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            structureTableController.RowSelected -= HandleStructureSelected;
            structureTableController.Submitted -= HandleStructureSubmitted;
            base.Unbind();
        }
        
        private void HandlePopulated(object? sender, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            ((NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController).Reset();
            ((NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController).Reset();
        }

        private void HandleStructureSelected(object? sender, ValueEventArgs<Structure?> e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.StructureTab.SetStructure(e.Element);
            ((NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController).Reset();
        }

        private void HandleStructureSubmitted(object? sender, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            var deltas = structureTableController.GetDeltas();
            if (deltas.Count > 0)
            {
                ((NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController).Reset();
                OrderCreated?.Invoke(this, new BuildOrder(pane.GetHolding(), deltas));
            }
        }
    }
}
