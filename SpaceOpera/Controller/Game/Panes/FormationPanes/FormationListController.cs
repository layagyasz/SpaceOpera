using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View.Game;
using SpaceOpera.View.Game.Panes.FormationPanes;

namespace SpaceOpera.Controller.Game.Panes.FormationPanes
{
    public class FormationListController : DynamicComponentControllerBase, IActionController
    {
        public EventHandler<EventArgs>? Closed { get; set; }
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public override void BindElement(IUiElement element)
        {
            var component = (IFormationComponent)element;
            var controller = (FormationComponentController)component.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        public override void UnbindElement(IUiElement element)
        {
            var component = (IFormationComponent)element;
            var controller = (FormationComponentController)component.ComponentController;
            controller.Interacted -= HandleInteraction;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.Action == ActionId.Unselect)
            {
                var row = _component!.Where(x => ((IFormationComponent)x).Key == e.GetOnlyObject()).FirstOrDefault();
                if (row != null)
                {
                    _component!.Remove(row);
                }
                if (_component!.Count == 0)
                {
                    Closed?.Invoke(this, EventArgs.Empty);
                }
            }
            Interacted?.Invoke(this, e);
        }
    }
}
