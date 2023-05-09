using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public interface IAssigner
    {
        AssignmentType Type { get; }
        ICollection<INavigable> GetActiveRegion();
        void SetActiveRegion(IEnumerable<INavigable> region);
        void Tick(SpaceOperaContext context);
    }
}
