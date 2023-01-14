using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    public class MoveNode : ISupplierNode<IAction, FleetContext>
    {
        private readonly ISupplierNode<INavigable, FleetContext> _destination;
        private readonly EnumSet<NavigableEdgeType> _allowedEdgeTypes;

        private Stack<NavigationMap.Movement>? _cachedPath;

        public MoveNode(
            ISupplierNode<INavigable, FleetContext> destination,
            EnumSet<NavigableEdgeType> allowedEdgeTypes)
        {
            _destination = destination;
            _allowedEdgeTypes = allowedEdgeTypes;
        }

        public BehaviorNodeResult<IAction> Execute(FleetContext context)
        {
            var currentPosition = context.Fleet.Fleet.Position!;
            var destination = _destination.Execute(context);
            if (!destination.Status.Complete)
            {
                return BehaviorNodeResult<IAction>.NotRun();
            }
            if (currentPosition == destination.Result || destination.Result == null)
            {
                return BehaviorNodeResult<IAction>.NotRun();
            }
            if (_cachedPath?.Peek().Destination == currentPosition)
            {
                _cachedPath!.Pop();
                if (_cachedPath?.Count == 0)
                {
                    _cachedPath = null;
                }
            }
            if (_cachedPath == null 
                || destination.Result != _cachedPath.Last().Destination
                || _cachedPath.Peek().Origin != currentPosition)
            {
                _cachedPath = 
                    context.World.NavigationMap.FindPath(currentPosition, destination.Result, _allowedEdgeTypes);
            }
            return BehaviorNodeResult<IAction>.Complete(new MoveAction(_cachedPath.Peek()));
        }
    }
}