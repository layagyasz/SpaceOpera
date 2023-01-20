using Cardamom.Utils.Suppliers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Orders;

namespace SpaceOpera
{
    public class GameDriver
    {
        public World World { get; }

        private readonly ITickable _tickable;

        public GameDriver(World world)
        {
            World = world;
            _tickable = world.GetTickable();
        }

        public ValidationFailureReason Execute(IOrder order)
        {
            var validation = order.Validate();
            if (validation != ValidationFailureReason.None)
            {
                return validation;
            }
            order.Execute(World);
            return ValidationFailureReason.None;
        }

        public void Start()
        {
            var thread = new Thread(TickThread);
            thread.Start();
        }

        private void TickThread()
        {
            var timer = new TimedSupplier<Action>(Tick, 1000);
            while (true)
            {
                timer.Get().Invoke();
            }
        }

        private void Tick()
        {
            _tickable.Tick();
        }
    }
}
