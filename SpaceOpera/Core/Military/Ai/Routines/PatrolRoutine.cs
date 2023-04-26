using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Routines
{
    public static class PatrolRoutine
    {
        private class PatrolNode : ISupplierNode<INavigable, FormationContext>
        {
            private INavigable? _cachedDestination;

            public BehaviorNodeResult<INavigable> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.Formation.Position;
                var activeRegion = context.Driver.GetActiveRegion();
                if (_cachedDestination == null
                    || _cachedDestination == context.Driver.Formation.Position
                    || !context.Driver.GetActiveRegion().Contains(_cachedDestination))
                {
                    var options = activeRegion.Where(x => x != context.Driver.Formation.Position).ToList();
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

        private class PatrolSpotNode : ISupplierNode<IAction, FormationContext>
        {
            private readonly ISupplierNode<Fleet, FormationContext> _target;

            public PatrolSpotNode(ISupplierNode<Fleet, FormationContext> target)
            {
                _target = target;
            }

            public BehaviorNodeResult<IAction> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.Formation.Position;
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

        private class PatrolTargetNode : ISupplierNode<IFormation, FormationContext>
        {
            private IFormation? _cachedTarget;

            public BehaviorNodeResult<IFormation> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.Formation.Position;
                var faction = context.Driver.Formation.Faction;
                var activeRegions = context.Driver.GetActiveRegion();
                if (_cachedTarget == null
                    || !activeRegions.Contains(_cachedTarget.Position!)
                    || !context.World.BattleManager.CanEngage(context.Driver.Formation, _cachedTarget))
                {
                    var options =
                        context.World.FormationManager.GetFleetDrivers()
                            .Where(x => x.Formation.Position == currentPosition)
                            .Where(x => activeRegions.Contains(x.Formation.Position!))
                            .Where(x => x.Formation.Faction != faction)
                            .Where(x => context.World.BattleManager.CanEngage(context.Driver.Formation, x.Formation))
                            .Select(x => x.Formation)
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
                    ? BehaviorNodeResult<IFormation>.Incomplete()
                    : BehaviorNodeResult<IFormation>.Complete(_cachedTarget);
            }
        }
        public static ISupplierNode<IAction, FormationContext> Create()
        {
            var targetBuffer = new PatrolTargetNode().Buffer();
            return new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun()) {
                targetBuffer.Recompute().Check(
                    (x, y) =>
                        y.Driver.Formation.Position == x.Position
                        && y.World.GetIntelligenceFor(y.Driver.Formation.Faction).FleetIntelligence.IsSpotted(x))
                    .Transform(EngageAction.Create),
                targetBuffer.Check((x, y) => y.Driver.Formation.Position == x.Position).Transform(SpotAction.Create),
                new MoveNode(
                   new SelectorNode<BehaviorNodeResult<INavigable>, FormationContext>(
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