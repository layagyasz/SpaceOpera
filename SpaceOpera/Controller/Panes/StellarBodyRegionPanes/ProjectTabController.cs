using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Panes.StellarBodyRegionPanes
{
    public class ProjectTabController : NoOpElementController<ProjectTab>, ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private IActionController? _projectController;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _projectController = (IActionController)_element!.ProjectTable.ComponentController;
            _projectController.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            _projectController!.Interacted -= HandleInteraction;
            base.Unbind();
        }

        public void Reset() { }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
