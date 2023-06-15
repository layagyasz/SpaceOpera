using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics.Projects
{
    public interface IProject : ITickable
    {
        string Name { get; }
        object Key { get; }
        Pool Progress { get; }
        ProjectStatus Status { get; }

        void Cancel();
        void Setup();
        void Finish();
    }
}
