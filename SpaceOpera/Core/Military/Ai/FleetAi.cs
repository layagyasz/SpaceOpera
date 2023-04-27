using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Assigments;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public class FleetAi : BaseFormationAi
    {
        public FleetAi()
            : base(new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.Incomplete())
                    {
                        new CheckContextNode<FormationContext>(x => x.Formation.InCombat)
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new CombatAction())),
                        new CheckContextNode<FormationContext>(x => !x.Formation.Cohesion.IsFull())
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new RegroupAction())),
                    }.Adapt(),
                  new PatrolAssignment())
        { }
    }
}