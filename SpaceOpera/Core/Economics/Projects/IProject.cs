using SpaceOpera.Core.Advanceable;

namespace SpaceOpera.Core.Economics.Projects
{
    public interface IProject : ITickable
    {
        void Setup();
        void Finish();
        bool IsDone();
    }
}
