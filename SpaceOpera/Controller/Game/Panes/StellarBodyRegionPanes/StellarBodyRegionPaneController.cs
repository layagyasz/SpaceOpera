using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes
{
    public class StellarBodyRegionPaneController : MultiTabGamePaneController
    {        
        protected override void HandlePopulatedImpl()
        {
            var pane = (StellarBodyRegionPane)_pane!;
            ((AutoMultiCountInputController<Structure>)pane.StructureTab.Structures.ComponentController).Reset();
            ((AutoMultiCountInputController<Recipe>)pane.StructureTab.Recipes.ComponentController).Reset();
        }
    }
}
