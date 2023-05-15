using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller
{
    public interface IOrderController
    {
        EventHandler<IOrder>? OrderCreated { get; set; }
    }
}
