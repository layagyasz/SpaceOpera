using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class NoAssigner : IAssigner
    {
        public AssignmentType Type => AssignmentType.None;

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>();
        }

        public void Tick(ICollection<AtomicFormationDriver> drivers, SpaceOperaContext context) { }
    }
}
