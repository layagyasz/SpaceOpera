namespace SpaceOpera.Core.Advanceable
{
    public class CycleTickable : ITickable
    {
        public int CycleLength { get; }

        private ITickable _tickable;
        private int _progress;

        public CycleTickable(ITickable tickable, int cycleLength)
        {
            CycleLength = cycleLength;
            _tickable = tickable;
        }

        public void Tick()
        {
            if (_progress++ == CycleLength)
            {
                _progress = 0;
                _tickable.Tick();
            }
        }
    }
}