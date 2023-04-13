using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller.Panes
{
    public interface IGamePaneController : IActionController, IPaneController
    {
        EventHandler<IOrder>? OrderCreated { get; set; }
    }
}
