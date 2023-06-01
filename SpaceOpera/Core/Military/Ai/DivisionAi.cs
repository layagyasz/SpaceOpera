using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public class DivisionAi : BaseFormationAi
    {
        public DivisionAi()
            : base(new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.Incomplete())
                    {
                        new CheckContextNode<FormationContext>(x => x.Driver.AtomicFormation.InCombat)
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new CombatAction())),
                        new CheckContextNode<FormationContext>(x => !x.Driver.AtomicFormation.Cohesion.IsFull())
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new RegroupAction())),
                    }.Adapt())
        { }
    }
}
