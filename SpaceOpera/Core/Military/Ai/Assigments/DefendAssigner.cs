using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class DefendAssigner : IAssigner
    {
        public AssignmentType Type => AssignmentType.Defend;

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>();
        }

        public void Tick(SpaceOperaContext context) { }
    }
}
