using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public abstract class BaseFormationAi : IFormationAi
    {
        protected readonly ISupplierNode<IAction, FormationContext> _routine;
        protected IAssignment _assignment;

        protected BaseFormationAi(ISupplierNode<IAction, FormationContext> routine)
        {
            _routine = routine;
            _assignment = new NoAssignment();
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            var result = _routine.Execute(context);
            return result.Status.Complete ? result : _assignment.Execute(context);
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
