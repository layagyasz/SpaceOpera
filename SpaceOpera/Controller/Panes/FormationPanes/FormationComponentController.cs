using SpaceOpera.View.Panes.FormationPanes;

namespace SpaceOpera.Controller.Panes.FormationPanes
{
    public class FormationComponentController : IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private IFormationComponent? _component;

        public void Bind(object @object)
        {
            _component = (IFormationComponent)@object;
            ((IActionController)_component.Header.ComponentController).Interacted += HandleInteraction;
            ((IActionController)_component.CompositionTable.ComponentController).Interacted += HandleInteraction;
        }

        public void Unbind()
        {
            ((IActionController)_component!.Header.ComponentController).Interacted -= HandleInteraction;
            ((IActionController)_component!.CompositionTable.ComponentController).Interacted -= HandleInteraction;
            _component = null;
            
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
