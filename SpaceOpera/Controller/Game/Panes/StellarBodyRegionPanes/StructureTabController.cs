using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes
{
    public class StructureTabController : NoOpElementController<StructureTab>, ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private AutoNumericInputTableController<Structure>? _structureController;
        private AutoNumericInputTableController<Recipe>? _recipeController;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _structureController = 
                (AutoNumericInputTableController<Structure>)_element!.Structures.ComponentController;
            _structureController.RowSelected += HandleStructureSelected;
            _element!.StructureSubmit.Controller.Clicked += HandleStructureSubmitted;
            _recipeController =
                (AutoNumericInputTableController<Recipe>)_element!.Recipes.ComponentController;
            _element!.RecipeSubmit.Controller.Clicked += HandleRecipeSubmitted;
        }

        public override void Unbind()
        {
            _structureController!.RowSelected -= HandleStructureSelected;
            _element!.StructureSubmit.Controller.Clicked -= HandleStructureSubmitted;
            _element!.RecipeSubmit.Controller.Clicked -= HandleRecipeSubmitted;
            base.Unbind();
        }

        public void Reset()
        {
            _structureController!.Reset();
            _recipeController!.Reset();
        }

        private void HandleStructureSelected(object? sender, Structure? e)
        {
            _element!.SetStructure(e);
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
