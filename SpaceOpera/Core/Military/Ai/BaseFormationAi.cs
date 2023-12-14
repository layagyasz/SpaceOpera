using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public abstract class BaseFormationAi : IFormationAi
    {
        protected readonly ISupplierNode<IAction, FormationContext> _preRoutine;
        protected readonly ISupplierNode<IAction, FormationContext> _postRoutine;
        protected IAssignment _assignment;

        protected BaseFormationAi(
            ISupplierNode<IAction, FormationContext> preRoutine, ISupplierNode<IAction, FormationContext> postRoutine)
        {
            _preRoutine = preRoutine;
            _postRoutine = postRoutine;
            _assignment = new NoAssignment();
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            var result = _preRoutine.Execute(context);
            if (!result.Status.Complete)
            {
                result = _assignment.Execute(context);
            }
            if (!result.Status.Complete)
            {
                result = _postRoutine.Execute(context);
            }
            if (!result.Status.Complete)
            {
                result = BehaviorNodeResult<IAction>.Complete(new IdleAction(unassign: false));
            }
            return result;
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _assignment.GetActiveRegion();
        }

        public IAssignment GetAssignment()
        {
            return _assignment;
        }

        public void SetActiveRegion(IEnumerable<INavigable> region)
        {
            _assignment.SetActiveRegion(region);
        }

        public void SetAssignment(IAssignment assignment)
        {
            assignment.SetActiveRegion(GetActiveRegion());
            _assignment = assignment;
        }
    }
}
