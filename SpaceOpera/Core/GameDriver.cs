using SpaceOpera.Core.Advanceable;
using System.Diagnostics;

namespace SpaceOpera.Core
{
    public class GameDriver
    {
        private int _gameSpeed = 1;
        private readonly IUpdateable _updater;

        public GameDriver(IUpdateable updater)
        {
            _updater = updater;
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
