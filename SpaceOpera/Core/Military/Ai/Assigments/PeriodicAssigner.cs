using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public abstract class PeriodicAssigner : IAssigner
    {
        public int Period { get; }
        public abstract AssignmentType Type { get; }

        private int _period = 0;

        public PeriodicAssigner(int period)
        {
            Period = period;
        }

        public abstract ICollection<INavigable> GetActiveRegion();
        public abstract void SetActiveRegion(IEnumerable<INavigable> region);

        public void Reset()
        {
            _period = 0;
        }
        
        public void Tick(ICollection<AtomicFormationDriver> drivers, SpaceOperaContext context)
        {
            if (--_period < 1)
            {
                TickImpl(drivers, context);
                _period = Period;
            }
        }

        protected abstract void TickImpl(ICollection<AtomicFormationDriver> drivers, SpaceOperaContext context);
    }
}
