using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class TimedProject : IProject
    {
        public IntPool Progress { get; }

        protected TimedProject(int time)
        {
            Progress = new(time);
        }

        public void Tick()
        {
            Progress.Change(1);
        }

        public bool IsDone()
        {
            return Progress.IsFull();
        }

        public abstract void Setup();
        public abstract void Finish();
    }
}
