namespace SpaceOpera.Core.Advanceable
{
    public class TickUpdateable : IUpdateable
    {
        private readonly ITickable _tickable;
        private readonly long _tick;

        private long _delta;

        public TickUpdateable(ITickable tickable, long tick)
        {
            _tickable = tickable;
            _tick = tick;
        }

        public void Update(long delta)
        {
            _delta += delta;
            while (_delta > _tick)
            {
                _tickable.Tick();
                _delta -= _tick;
            }
        }
    }
}
