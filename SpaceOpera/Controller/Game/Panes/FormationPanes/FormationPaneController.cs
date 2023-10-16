using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.FormationPanes;

namespace SpaceOpera.Controller.Game.Panes.FormationPanes
{
    public class FormationPaneController : PaneController, IGamePaneController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IInterceptor>? InterceptorCreated { get; set; }
        public EventHandler<IInterceptor>? InterceptorCancelled { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        protected FormationPane? _pane;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = (FormationPane)@object;
            var listController = (FormationListController)_pane!.FormationList.ComponentController;
            listController.Closed += HandleClose;
            listController.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var listController = (FormationListController)_pane!.FormationList.ComponentController;
            listController.Closed -= HandleClose;
            listController.Interacted -= HandleInteraction;
            base.Unbind();
        }

        private void HandleClose(object? sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
