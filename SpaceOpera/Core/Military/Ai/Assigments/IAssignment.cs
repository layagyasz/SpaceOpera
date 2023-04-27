using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public interface IAssignment : ISupplierNode<IAction, FormationContext>
    {
        ICollection<INavigable> GetActiveRegion();
        void SetActiveRegion(IEnumerable<INavigable> region);
    }
}
