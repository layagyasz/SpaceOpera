using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class TrainAssignment : IAssignment
    {
        public AssignmentType Type => AssignmentType.Train;

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>();
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return BehaviorNodeResult<IAction>.Complete(new TrainAction());
        }
    }
}
