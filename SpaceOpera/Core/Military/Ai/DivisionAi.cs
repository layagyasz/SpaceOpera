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
                        new CheckContextNode<FormationContext>(
                            x => x.Driver.AtomicFormation.Cohesion.IsEmpty() 
                            && x.World.Battles.CanDefend(x.Driver.AtomicFormation))
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(RetreatAction.Create())),
                        new CheckContextNode<FormationContext>(x => x.Driver.AtomicFormation.InCombat > 0)
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new CombatAction())),
                    }.Adapt(),
                  new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.Incomplete())
                    {
                        new CheckContextNode<FormationContext>(
                            x => x.World.Battles.CanDefend(x.Driver.AtomicFormation))
                            .AndThen(SourceNode<IAction, FormationContext>.Wrap(new DefendAction()))
                    }.Adapt())
        { }
    }
}
