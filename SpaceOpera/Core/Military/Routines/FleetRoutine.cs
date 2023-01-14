using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Actions;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    public class FleetRoutine
    {
        public static ISupplierNode<IAction, FleetContext> Create()
        {
            return new SelectorNode<BehaviorNodeResult<IAction>, FleetContext>(
                x => x.Status.Complete, BehaviorNodeResult<IAction>.Incomplete())
            {
                new CheckContextNode<FleetContext>(x => x.Fleet.Fleet.InCombat)
                    .AndThen(SourceNode<IAction, FleetContext>.Wrap(new CombatAction())),
                new CheckContextNode<FleetContext>(x => !x.Fleet.Fleet.Cohesion.IsFull())
                    .AndThen(SourceNode<IAction, FleetContext>.Wrap(new RegroupAction())),
                PatrolRoutine.Create()
            }.Adapt();
        }
    }
}