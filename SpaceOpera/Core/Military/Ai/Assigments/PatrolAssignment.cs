using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class PatrolAssignment : IAssignment
    {
        private class PatrolNode : ISupplierNode<INavigable?, FormationContext>
        {
            private readonly PatrolAssignment _parent;

            private INavigable? _cachedDestination;

            public PatrolNode(PatrolAssignment parent)
            {
                _parent = parent;
            }

            public BehaviorNodeResult<INavigable?> Execute(FormationContext context)
            {
                var currentPosition = context.Formation.Position;
                var activeRegion = _parent.GetActiveRegion();
                if (_cachedDestination == null
                    || _cachedDestination == context.Formation.Position
                    || !_parent.GetActiveRegion().Contains(_cachedDestination))
                {
                    var options = activeRegion.Where(x => x != context.Formation.Position).ToList();
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
                    ? BehaviorNodeResult<INavigable?>.Incomplete()
                    : BehaviorNodeResult<INavigable?>.Complete(_cachedDestination);
            }
        }

        private class PatrolTargetNode : ISupplierNode<IFormation, FormationContext>
        {
            private readonly PatrolAssignment _parent;

            private IFormation? _cachedTarget;

            public PatrolTargetNode(PatrolAssignment parent)
            {
                _parent = parent;
            }

            public BehaviorNodeResult<IFormation> Execute(FormationContext context)
            {
                var currentPosition = context.Formation.Position;
                var faction = context.Formation.Faction;
                var activeRegions = _parent.GetActiveRegion();
                if (_cachedTarget == null
                    || !activeRegions.Contains(_cachedTarget.Position!)
                    || !context.World.BattleManager.CanEngage(context.Formation, _cachedTarget))
                {
                    var options =
                        context.World.FormationManager.GetFleetDrivers()
                            .Where(x => x.Formation.Position == currentPosition)
                            .Where(x => activeRegions.Contains(x.Formation.Position!))
                            .Where(x => x.Formation.Faction != faction)
                            .Where(x => context.World.BattleManager.CanEngage(context.Formation, x.Formation))
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

        public AssignmentType Type => AssignmentType.Patrol;

        private readonly ISupplierNode<IAction, FormationContext> _routine;

        private HashSet<INavigable> _patrolRegion = new();

        public PatrolAssignment()
        {
            var targetBuffer = new PatrolTargetNode(this).Buffer();
            _routine =
                new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun())
                {
                    targetBuffer.Recompute().Check(
                        (x, y) =>
                            y.Formation.Position == x.Position
                            && y.World.GetIntelligenceFor(y.Formation.Faction).FleetIntelligence.IsSpotted(x))
                        .Transform(EngageAction.Create),
                    targetBuffer.Check((x, y) => y.Formation.Position == x.Position).Transform(SpotAction.Create),
                    new MoveNode(
                       new SelectorNode<BehaviorNodeResult<INavigable?>, FormationContext>(
                           x => x.Status.Complete, BehaviorNodeResult<INavigable?>.Incomplete())
                       {
                           targetBuffer.Transform(x => x.Position),
                           new PatrolNode(this),
                       }.Adapt(),
                       new EnumSet<NavigableEdgeType>(NavigableEdgeType.Space, NavigableEdgeType.Jump))
                }.Adapt();
        }

        public void SetActiveRegion(IEnumerable<INavigable> region)
        {
            _patrolRegion = new(region);
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _patrolRegion;
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }
    }
}