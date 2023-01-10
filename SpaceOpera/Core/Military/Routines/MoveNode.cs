using Adansonia;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    class MoveNode : ISupplierNode<IAction, FleetContext>
    {
        private readonly ISupplierNode<INavigable, FleetContext> _Destination;
        private readonly EnumSet<NavigableEdgeType> _AllowedEdgeTypes;

        private Stack<NavigationMap.Movement> _CachedPath;

        public MoveNode(
            ISupplierNode<INavigable, FleetContext> Destination,
            EnumSet<NavigableEdgeType> AllowedEdgeTypes)
        {
            _Destination = Destination;
            _AllowedEdgeTypes = AllowedEdgeTypes;
        }

        public AdansoniaNodeResult<IAction> Execute(FleetContext Context)
        {
            var currentPosition = Context.Fleet.Fleet.Position;
            var destination = _Destination.Execute(Context);
            if (!destination.Status.Complete)
            {
                return AdansoniaNodeResult<IAction>.NotRun();
            }
            if (currentPosition == destination.Result || destination.Result == null)
            {
                return AdansoniaNodeResult<IAction>.NotRun();
            }
            if (_CachedPath?.Peek().Destination == currentPosition)
            {
                _CachedPath.Pop();
                if (_CachedPath?.Count == 0)
                {
                    _CachedPath = null;
                }
            }
            if (_CachedPath == null 
                || destination.Result != _CachedPath.Last().Destination
                || _CachedPath.Peek().Origin != currentPosition)
            {
                _CachedPath = 
                    Context.World.NavigationMap.FindPath(currentPosition, destination.Result, _AllowedEdgeTypes);
            }
            return AdansoniaNodeResult<IAction>.Complete(new MoveAction(_CachedPath.Peek()));
        }
    }
}