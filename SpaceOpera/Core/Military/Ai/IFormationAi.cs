using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai
{
    public interface IFormationAi : ISupplierNode<IAction, FormationContext>
    {
    }
}
