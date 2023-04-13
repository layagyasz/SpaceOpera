using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.View.Panes.FormationPanes;

namespace SpaceOpera.Controller.Panes.FormationPanes
{
    public class FormationListController : DynamicComponentControllerBase, IActionController
    {
        public EventHandler<EventArgs>? Closed { get; set; }
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        public override void BindElement(IUiElement element)
        {
            var component = (FormationComponent)element;
            var controller = (FormationComponentController)component.ComponentController;
            controller.Interacted += HandleInteraction;
        }

        public override void UnbindElement(IUiElement element)
        {
            var component = (FormationComponent)element;
            var controller = (FormationComponentController)component.ComponentController;
            controller.Interacted -= HandleInteraction;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.Action == View.ActionId.Unselect)
            {
                var row = _component!.Where(x => ((FormationComponent)x).Key == e.GetOnlyObject()).FirstOrDefault();
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
