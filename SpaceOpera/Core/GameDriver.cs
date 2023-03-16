using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Orders;
using System.Diagnostics;

namespace SpaceOpera.Core
{
    public class GameDriver
    {
        public World? World { get; }

        private int _gameSpeed = 1;
        private readonly IUpdateable _updater;

        public GameDriver(World? world, IUpdateable updater)
        {
            World = world;
            _updater = updater;
        }

        public ValidationFailureReason Execute(IOrder order)
        {
            var validation = order.Validate();
            if (validation != ValidationFailureReason.None)
            {
                return validation;
            }
            order.Execute(World!);
            return ValidationFailureReason.None;
        }

        public void SetGameSpeed(int gameSpeed)
        {
            _gameSpeed = gameSpeed;
        }

        public void Start()
        {
            var thread = new Thread(TickThread);
            thread.Start();
        }

        private void TickThread()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long elapsed = 0;
            while (true)
            {
                long frameElapsed = stopwatch.ElapsedMilliseconds;
                long delta = frameElapsed - elapsed;
                _updater.Update(_gameSpeed * delta);
                elapsed = frameElapsed;
            }
        }
    }
}
