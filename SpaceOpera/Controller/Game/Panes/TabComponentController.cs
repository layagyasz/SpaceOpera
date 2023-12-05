using Cardamom.Ui.Controller;
using Cardamom.Ui;
using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller.Game.Panes
{
    public class TabComponentController : DynamicComponentControllerBase, ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        public void Reset() { }

        protected override void BindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                if (row.ComponentController is IActionController action)
                {
                    action!.Interacted += HandleInteraction;
                }
                if (row.ComponentController is IOrderController order)
                {
                    order!.OrderCreated += HandleOrder;
                }
            }
        }

        protected override void UnbindElement(IUiElement element)
        {
            if (element is IUiComponent row)
            {
                if (row.ComponentController is IActionController action)
                {
                    action!.Interacted -= HandleInteraction;
                }
                if (row.ComponentController is IOrderController order)
                {
                    order!.OrderCreated -= HandleOrder;
                }
            }
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }

        private void HandleOrder(object? sender, IOrder order)
        {
            OrderCreated?.Invoke(this, order);
        }
    }
}
