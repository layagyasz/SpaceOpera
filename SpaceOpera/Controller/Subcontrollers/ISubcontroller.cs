using SpaceOpera.Core.Orders;

namespace SpaceOpera.Controller.Subcontrollers
{
    public interface ISubcontroller
    {
        EventHandler<IOrder>? OrderCreated { get; set; }

        bool HandleInteraction(UiInteractionEventArgs interaction);
    }
}
