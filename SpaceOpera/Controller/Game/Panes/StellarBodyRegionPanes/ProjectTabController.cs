using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Orders;
using SpaceOpera.View;
using SpaceOpera.View.Game.Panes.StellarBodyRegionPanes;

namespace SpaceOpera.Controller.Game.Panes.StellarBodyRegionPanes
{
    public class ProjectTabController : ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private ProjectTab? _tab;
        private IActionController? _projectController;

        public void Bind(object @object)
        {
            _tab = (ProjectTab)@object;
            _projectController = (IActionController)_tab!.Projects.ComponentController;
            _projectController.Interacted += HandleInteraction;
        }

        public void Unbind()
        {
            _projectController!.Interacted -= HandleInteraction;
            _projectController = null;
            _tab = null;
        }

        public void Reset() { }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is IProject project)
            {
                if (e.Action == ActionId.Cancel)
                {
                    OrderCreated?.Invoke(this, new CancelProjectOrder(project));
                }
            }
        }
    }
}
