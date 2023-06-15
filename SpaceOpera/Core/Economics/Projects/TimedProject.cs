using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public abstract class TimedProject : BaseProject
    {
        public override Pool Progress { get; }

        protected TimedProject(float time)
        {
            Progress = new(time, /* startFull= */ false);
        }

        protected override void TickImpl()
        {
            Progress.Change(1f);
        }
    }
}
