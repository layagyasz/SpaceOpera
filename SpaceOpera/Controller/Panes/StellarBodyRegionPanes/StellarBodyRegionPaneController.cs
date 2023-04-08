using Cardamom.Ui.Controller.Element;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
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
        }

        public override void Unbind()
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.Populated -= HandlePopulated;
            var structureTableController =
                (NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController;
            structureTableController.RowSelected -= HandleStructureSelected;
            base.Unbind();
        }
        
        private void HandlePopulated(object? @object, EventArgs e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            ((NumericInputTableController<Structure>)pane.StructureTab.StructureTable.ComponentController).Reset();
            ((NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController).Reset();
        }

        private void HandleStructureSelected(object? @object, ValueEventArgs<Structure?> e)
        {
            var pane = (StellarBodyRegionPane)_pane!;
            pane.StructureTab.SetStructure(e.Element);
            ((NumericInputTableController<Recipe>)pane.StructureTab.RecipeTable.ComponentController).Reset();
            ((TableController)pane.StructureTab.RecipeTable.Table.Controller).Reset();
        }
    }
}
