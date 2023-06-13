using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics.Projects
{
    public interface IProject : ITickable
    {
        string Name { get; }
        object Key { get; }

        void Cancel();
        void Setup();
        void Finish();
        bool IsDone();
    }
}
