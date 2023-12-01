using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations.Assignments;
using SpaceOpera.View.Game;
using SpaceOpera.View.Game.Panes.FormationPanes;
using SpaceOpera.View;
using SpaceOpera.Core.Orders.Formations;

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
            var assigment =
                e.Action == null ? AssignmentType.Unknown : ActionIdMapper.ToAssignmentType(e.Action.Value);
            if (assigment != AssignmentType.Unknown)
            {
                foreach (var driver in e.Objects.Cast<IFormationDriver>())
                {
                    OrderCreated?.Invoke(this, new SetAssignmentOrder(driver, assigment));
                }
            }
            else if (e.Action == ActionId.Split)
            {
                if (e.GetOnlyObject() is DivisionDriver division)
                {
                    OrderCreated?.Invoke(this, new LeaveArmyOrder(division));
                }
            }
            else 
            {
                Interacted?.Invoke(this, e);
            }
        }
    }
}
