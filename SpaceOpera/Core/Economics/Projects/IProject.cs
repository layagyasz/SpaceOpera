using Cardamom.Trackers;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics.Projects
{
    public interface IProject : ITickable
    {
        string Name { get; }
        object Key { get; }
        public Faction Faction { get; }
        Pool Progress { get; }
        ProjectStatus Status { get; }

        void Cancel();
        void Setup();
        void Finish(World world);
    }
}
