using SpaceOpera.View.Panes.FormationPanes;

namespace SpaceOpera.Controller.Panes.FormationPanes
{
    public class FormationComponentController : IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private FormationComponent? _component;

        public void Bind(object @object)
        {
            _component = (FormationComponent)@object;
            ((IActionController)_component.Header.ComponentController).Interacted += HandleInteraction;
            ((IActionController)_component.UnitGroupingTable.ComponentController).Interacted += HandleInteraction;
        }

        public void Unbind()
        {
            ((IActionController)_component!.Header.ComponentController).Interacted -= HandleInteraction;
            ((IActionController)_component!.UnitGroupingTable.ComponentController).Interacted -= HandleInteraction;
            _component = null;
            
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
