using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class TimedProject : IProject
    {
        public abstract object Key { get; }
        public abstract string Name { get; }

        public IntPool Progress { get; }

        protected TimedProject(int time)
        {
            Progress = new(time, /* startFull= */ false);
        }

        public void Tick()
        {
            Progress.Change(1);
        }

        public bool IsDone()
        {
            return Progress.IsFull();
        }

        public abstract void Cancel();
        public abstract void Setup();
        public abstract void Finish();
    }
}
