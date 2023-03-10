using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    public static class PatrolRoutine
    {
        private class PatrolNode : ISupplierNode<INavigable, FleetContext>
        {
            private INavigable? _cachedDestination;

            public BehaviorNodeResult<INavigable> Execute(FleetContext context)
            {
                var currentPosition = context.Fleet.Fleet.Position;
                var activeRegion = context.Fleet.GetActiveRegion();
                if (_cachedDestination == null 
                    || _cachedDestination == context.Fleet.Fleet.Position 
                    || !context.Fleet.GetActiveRegion().Contains(_cachedDestination))
                {
                    var options = activeRegion.Where(x => x != context.Fleet.Fleet.Position).ToList();
                    if (options.Count == 0)
                    {
                        _cachedDestination = null;
                    }
                    else if (options.Count == 1)
                    {
                        _cachedDestination = options[0];
                    }
                    else
                    {
                        _cachedDestination = options[context.World.Random.Next(0, options.Count)];
                    }
                }
                return _cachedDestination == null 
                    ? BehaviorNodeResult<INavigable>.Incomplete()
                    : BehaviorNodeResult<INavigable>.Complete(_cachedDestination);
            }
        }

        private class PatrolSpotNode : ISupplierNode<IAction, FleetContext>
        {
            private readonly ISupplierNode<Fleet, FleetContext> _target;

            public PatrolSpotNode(ISupplierNode<Fleet, FleetContext> target)
            {
                _target = target;
            }

            public BehaviorNodeResult<IAction> Execute(FleetContext context)
            {
                var currentPosition = context.Fleet.Fleet.Position;
                var target = _target.Execute(context);
                if (!target.Status.Complete)
                {
                    return BehaviorNodeResult<IAction>.NotRun();
                }
                if (target.Result!.Position == currentPosition)
                {
                    return BehaviorNodeResult<IAction>.Complete(new SpotAction(target.Result));
                }
                return BehaviorNodeResult<IAction>.Incomplete();
            }
        }

        private class PatrolTargetNode : ISupplierNode<Fleet, FleetContext>
        {
            private Fleet? _cachedTarget;

            public BehaviorNodeResult<Fleet> Execute(FleetContext context)
            {
                var currentPosition = context.Fleet.Fleet.Position;
                var faction = context.Fleet.Fleet.Faction;
                var activeRegions = context.Fleet.GetActiveRegion();
                if (_cachedTarget == null 
                    || !activeRegions.Contains(_cachedTarget.Position) 
                    || !context.World.BattleManager.CanEngage(context.Fleet.Fleet, _cachedTarget))
                {
                    var options =
                        context.World.GetFleets()
                            .Where(x => x.Position == currentPosition)
                            .Where(x => activeRegions.Contains(x.Position))
                            .Where(x => x.Faction != faction)
                            .Where(x => context.World.BattleManager.CanEngage(context.Fleet.Fleet, x))
                            .ToList();
                    if (options.Count == 0)
                    {
                        _cachedTarget = null;
                    }
                    else if (options.Count == 1)
                    {
                        _cachedTarget = options[0];
                    }
                    else
                    {
                        _cachedTarget = options[context.World.Random.Next(0, options.Count)];
                    }
                }
                return _cachedTarget == null
                    ? BehaviorNodeResult<Fleet>.Incomplete()
                    : BehaviorNodeResult<Fleet>.Complete(_cachedTarget);
            }
        }
        public static ISupplierNode<IAction, FleetContext> Create()
        {
            var targetBuffer = new PatrolTargetNode().Buffer();
            return new SelectorNode<BehaviorNodeResult<IAction>, FleetContext>(
                x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun()) {
                targetBuffer.Recompute().Check(
                    (x, y) =>
                        y.Fleet.Fleet.Position == x.Position 
                        && y.World.GetIntelligenceFor(y.Fleet.Fleet.Faction).FleetIntelligence.IsSpotted(x))
                    .Transform(EngageAction.Create),
                targetBuffer.Check((x, y) => y.Fleet.Fleet.Position == x.Position).Transform(SpotAction.Create),
                new MoveNode(
                   new SelectorNode<BehaviorNodeResult<INavigable>, FleetContext>(
                       x => x.Status.Complete, BehaviorNodeResult<INavigable>.Incomplete())
                   {
                       targetBuffer.Transform(x => x.Position!),
                       new PatrolNode(),
                   }.Adapt(),
                   new EnumSet<NavigableEdgeType>(NavigableEdgeType.Space, NavigableEdgeType.Jump))
            }.Adapt();
        }
    }
}