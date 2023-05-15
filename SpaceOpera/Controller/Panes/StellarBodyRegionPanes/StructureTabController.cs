using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Panes.StellarBodyRegionPanes
{
    public class StructureTabController : NoOpElementController<StructureTab>, ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private NumericInputTableController<Structure>? _structureController;
        private NumericInputTableController<Recipe>? _recipeController;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _structureController = 
                (NumericInputTableController<Structure>)_element!.StructureTable.ComponentController;
            _structureController.RowSelected += HandleStructureSelected;
            _element!.StructureSubmit.Controller.Clicked += HandleStructureSubmitted;
            _recipeController =
                (NumericInputTableController<Recipe>)_element!.RecipeTable.ComponentController;
            _element!.RecipeSubmit.Controller.Clicked += HandleRecipeSubmitted;
        }

        public override void Unbind()
        {
            base.Unbind();
        }

        public void Reset()
        {
            _structureController!.Reset();
            _recipeController!.Reset();
        }

        private void HandleStructureSelected(object? sender, ValueEventArgs<Structure?> e)
        {
            _element!.SetStructure(e.Element);
            _recipeController!.Reset();
        }

        private void HandleRecipeSubmitted(object? sender, MouseButtonClickEventArgs e)
        {
            var deltas = _recipeController!.GetDeltas();
            if (deltas.Count > 0)
            {
                _recipeController.Reset();
                OrderCreated?.Invoke(
                    this,
                    new AdjustProductionOrder(_element!.GetHolding()!, _structureController!.GetSelected(), deltas));
            }
        }

        private void HandleStructureSubmitted(object? sender, MouseButtonClickEventArgs e)
        {
            var deltas = _structureController!.GetDeltas();
            if (deltas.Count > 0)
            {
                _structureController!.Reset();
                OrderCreated?.Invoke(this, new BuildOrder(_element!.GetHolding()!, deltas));
            }
        }
    }
}
