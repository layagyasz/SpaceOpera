using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Routines;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public class FleetAi : IFormationAi
    {
        private readonly ISupplierNode<IAction, FormationContext> _routine;

        public FleetAi()
        {
            _routine =
                new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.Incomplete())
                    {
                                    new CheckContextNode<FormationContext>(x => x.Driver.Formation.InCombat)
                                        .AndThen(SourceNode<IAction, FormationContext>.Wrap(new CombatAction())),
                                    new CheckContextNode<FormationContext>(x => !x.Driver.Formation.Cohesion.IsFull())
                                        .AndThen(SourceNode<IAction, FormationContext>.Wrap(new RegroupAction())),
                                    PatrolRoutine.Create()
                    }.Adapt();
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }
    }
}