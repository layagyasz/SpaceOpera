using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public class DivisionAi : IFormationAi
    {
        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return BehaviorNodeResult<IAction>.Complete(new IdleAction());
        }
    }
}
