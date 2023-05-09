using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public interface IFormationDriver
    {
        EventHandler<EventArgs>? OrderUpdated { get; set; }
        IFormation Formation { get; }
        AssignmentType GetAssignment();
        void SetAssignment(AssignmentType type);
        ICollection<INavigable> GetActiveRegion();
        void SetActiveRegion(IEnumerable<INavigable> activeRegion);
        void Tick(SpaceOperaContext context);
    }
}
