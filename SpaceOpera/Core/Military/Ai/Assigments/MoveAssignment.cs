using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class MoveAssignment : IAssignment
    {
        public bool IsHighPriority => false;
        public AssignmentType Type => AssignmentType.Move;

        private readonly ISupplierNode<IAction, FormationContext> _routine;

        private INavigable? _destination;

        public MoveAssignment()
        {
            _routine =
                new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun())
                {
                    new MoveNode(
                        SourceNode<INavigable?, FormationContext>.Wrap(GetDestination),
                        new(NavigableEdgeType.Ground), 
                        autoAttack: true),
                    SourceNode<IAction, FormationContext>.Wrap(new IdleAction(unassign: true))
                }.Adapt();
        }

        public void SetActiveRegion(IEnumerable<INavigable> region)
        {
            _destination = region.FirstOrDefault();
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            if (_destination == null)
            {
                return new List<INavigable>();
            }
            return new List<INavigable>() { _destination };
        }

        public INavigable? GetDestination()
        {
            return _destination;
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }
    }
}
