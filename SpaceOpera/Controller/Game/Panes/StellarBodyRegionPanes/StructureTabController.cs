using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Controller.Components.NumericInputs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes
{
    public class StructureTabController : ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private StructureTab? _tab;
        private AutoMultiCountInputController<Structure>? _structureController;
        private AutoMultiCountInputController<Recipe>? _recipeController;

        public void Bind(object @object)
        {
            _tab = (StructureTab)@object;
            _structureController = 
                (AutoMultiCountInputController<Structure>)_tab!.Structures.ComponentController;
            _structureController.RowSelected += HandleStructureSelected;
            _tab!.StructureSubmit.Controller.Clicked += HandleStructureSubmitted;
            _recipeController =
                (AutoMultiCountInputController<Recipe>)_tab!.Recipes.ComponentController;
            _tab!.RecipeSubmit.Controller.Clicked += HandleRecipeSubmitted;
        }

        public void Unbind()
        {
            _structureController!.RowSelected -= HandleStructureSelected;
            _tab!.StructureSubmit.Controller.Clicked -= HandleStructureSubmitted;
            _tab!.RecipeSubmit.Controller.Clicked -= HandleRecipeSubmitted;
            _tab = null;
        }

        public void Reset()
        {
            _structureController!.Reset();
            _recipeController!.Reset();
        }

        private void HandleStructureSelected(object? sender, Structure? e)
        {
            _tab!.SetStructure(e);
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
                    new AdjustProductionOrder(_tab!.GetHolding()!, _structureController!.GetSelected(), deltas));
            }
        }

        private void HandleStructureSubmitted(object? sender, MouseButtonClickEventArgs e)
        {
            var deltas = _structureController!.GetDeltas();
            if (deltas.Count > 0)
            {
                _structureController!.Reset();
                OrderCreated?.Invoke(this, new BuildOrder(_tab!.GetHolding()!, deltas));
            }
        }
    }
}
