using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes
{
    public class ProjectTabController : NoOpElementController<ProjectTab>, ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private IActionController? _projectController;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _projectController = (IActionController)_element!.Projects.ComponentController;
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
