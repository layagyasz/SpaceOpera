using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Routines
{
    public class MoveNode : ISupplierNode<IAction, FormationContext>
    {
        private readonly ISupplierNode<INavigable?, FormationContext> _destination;
        private readonly EnumSet<NavigableEdgeType> _allowedEdgeTypes;

        private Stack<NavigationMap.Movement>? _cachedPath;

        public MoveNode(
            ISupplierNode<INavigable?, FormationContext> destination,
            EnumSet<NavigableEdgeType> allowedEdgeTypes)
        {
            _destination = destination;
            _allowedEdgeTypes = allowedEdgeTypes;
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            var currentPosition = context.Driver.AtomicFormation.Position!;
            var destination = _destination.Execute(context);
            if (!destination.Status.Complete)
            {
                return BehaviorNodeResult<IAction>.NotRun();
            }
            if (currentPosition == destination.Result || destination.Result == null)
            {
                return BehaviorNodeResult<IAction>.NotRun();
            }
            bool empty = _cachedPath?.Count == 0;
            if (!empty && _cachedPath?.Peek().Destination == currentPosition)
            {
                _cachedPath!.Pop();
                if (_cachedPath?.Count == 0)
                {
                    _cachedPath = null;
                }
            }
            if (_cachedPath == null
                || (!empty 
                    && (destination.Result != _cachedPath.Last().Destination 
                        || _cachedPath.Peek().Origin != currentPosition)))
            {
                _cachedPath =
                    context.World.NavigationMap.FindPath(currentPosition, destination.Result, _allowedEdgeTypes);
            }
            return _cachedPath!.Count == 0 
                ? BehaviorNodeResult<IAction>.NotRun() 
                : BehaviorNodeResult<IAction>.Complete(new MoveAction(_cachedPath!.Peek()));
        }
    }
}