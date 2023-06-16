using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller.Game
{
    public interface IOrderController
    {
        EventHandler<IOrder>? OrderCreated { get; set; }
    }
}
