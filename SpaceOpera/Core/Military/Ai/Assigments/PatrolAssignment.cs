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
                var currentPosition = context.Driver.AtomicFormation.Position;
                var activeRegion = _parent.GetActiveRegion();
                if (_cachedDestination == null
                    || _cachedDestination == context.Driver.AtomicFormation.Position
                    || !_parent.GetActiveRegion().Contains(_cachedDestination))
                {
                    var options = activeRegion.Where(x => x != context.Driver.AtomicFormation.Position).ToList();
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

        private class PatrolTargetNode : ISupplierNode<IAtomicFormation, FormationContext>
        {
            private readonly PatrolAssignment _parent;

            private IAtomicFormation? _cachedTarget;

            public PatrolTargetNode(PatrolAssignment parent)
            {
                _parent = parent;
            }

            public BehaviorNodeResult<IAtomicFormation> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.AtomicFormation.Position;
                var faction = context.Driver.AtomicFormation.Faction;
                var activeRegions = _parent.GetActiveRegion();
                if (!activeRegions.Contains(currentPosition!))
                {
                    return BehaviorNodeResult<IAtomicFormation>.Incomplete();
                }
                if (_cachedTarget == null
                    || !activeRegions.Contains(_cachedTarget.Position!)
                    || !context.World.Battles.CanEngage(context.Driver.AtomicFormation, _cachedTarget))
                {
                    var options =
                        context.World.Formations.GetFormationsIn(currentPosition!)
                            .Where(x => context.World.Battles.CanEngage(
                                context.Driver.AtomicFormation, x.AtomicFormation))
                            .Select(x => x.AtomicFormation)
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
                    ? BehaviorNodeResult<IAtomicFormation>.Incomplete()
                    : BehaviorNodeResult<IAtomicFormation>.Complete(_cachedTarget);
            }
        }

        public bool IsHighPriority => false;
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
                            y.Driver.AtomicFormation.Position == x.Position
                            && y.World.Intelligence.Get(y.Driver.Formation.Faction).FleetIntelligence.IsSpotted(x))
                        .Transform(EngageAction.Create),
                    targetBuffer.Check(
                        (x, y) => y.Driver.AtomicFormation.Position == x.Position).Transform(SpotAction.Create),
                    new MoveNode(
                       new SelectorNode<BehaviorNodeResult<INavigable?>, FormationContext>(
                           x => x.Status.Complete, BehaviorNodeResult<INavigable?>.Incomplete())
                       {
                           targetBuffer.Transform(x => x.Position),
                           new PatrolNode(this),
                       }.Adapt(),
                       new EnumSet<NavigableEdgeType>(NavigableEdgeType.Space, NavigableEdgeType.Jump),
                       autoAttack: false)
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