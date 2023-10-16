using Cardamom.Ui;
using SpaceOpera.View;
using SpaceOpera.View.Game.Panes.OrderConfirmationPanes;

namespace SpaceOpera.Controller.Game.Panes.OrderConfirmationPanes
{
    public class OrderConfirmationPaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = (OrderConfirmationPane)_pane!;
            pane.Confirm.Controller.Clicked += HandleConfirmed;
        }

        public override void Unbind()
        {
            var pane = (OrderConfirmationPane)_pane!;
            pane.Confirm.Controller.Clicked -= HandleConfirmed;
            base.Unbind();
        }

        private void HandleConfirmed(object? sender, MouseButtonClickEventArgs e)
        {
            var pane = (OrderConfirmationPane)_pane!;
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(pane.GetOrder(), ActionId.Confirm));
            Close();
        }
    }
}
