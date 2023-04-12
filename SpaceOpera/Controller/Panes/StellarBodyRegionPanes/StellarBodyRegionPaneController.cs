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

            var recipeTableController =
                (NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController;
            recipeTableController.Submitted += HandleRecipeSubmitted;
        }

        public override void Unbind()
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.Populated -= HandlePopulated;

            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            structureTableController.RowSelected -= HandleStructureSelected;
            structureTableController.Submitted -= HandleStructureSubmitted;

            var recipeTableController =
                (NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController;
            recipeTableController.Submitted -= HandleRecipeSubmitted;

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

        private void HandleRecipeSubmitted(object? sender, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            var recipeTableController =
                (NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController;
            var deltas = recipeTableController.GetDeltas();
            if (deltas.Count > 0)
            {
                recipeTableController.Reset();
                OrderCreated?.Invoke(
                    this, 
                    new AdjustProductionOrder(pane.GetHolding(), structureTableController.GetSelected(), deltas));
            }
        }

        private void HandleStructureSubmitted(object? sender, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            var deltas = structureTableController.GetDeltas();
            if (deltas.Count > 0)
            {
                structureTableController.Reset();
                OrderCreated?.Invoke(this, new BuildOrder(pane.GetHolding(), deltas));
            }
        }
    }
}
