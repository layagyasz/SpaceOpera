using Cardamom.Ui.Controller;
using Cardamom.Ui;
using SpaceOpera.Core.Orders;
using Cardamom.Utils;

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

        protected virtual Optional<IOrder> InterceptInteraction(UiInteractionEventArgs e)
        {
            return Optional<IOrder>.Empty();
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            var intercepted = InterceptInteraction(e);
            if (intercepted.HasValue)
            {
                OrderCreated?.Invoke(this, intercepted.OrElseDefault()!);
            }
            else
            {
                Interacted?.Invoke(this, e);
            }
        }

        private void HandleOrder(object? sender, IOrder order)
        {
            OrderCreated?.Invoke(this, order);
        }
    }
}
