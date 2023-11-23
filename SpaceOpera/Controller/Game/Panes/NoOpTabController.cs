using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller.Game.Panes
{
    public class NoOpTabController : ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        public void Bind(object @object) { }

        public void Unbind() { }

        public void Reset() { }
    }
}
