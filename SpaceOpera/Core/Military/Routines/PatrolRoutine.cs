using Adansonia;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Routines
{
    static class PatrolRoutine
    {
        private class PatrolNode : ISupplierNode<INavigable, FleetContext>
        {
            private INavigable _CachedDestination;

            public AdansoniaNodeResult<INavigable> Execute(FleetContext Context)
            {
                var currentPosition = Context.Fleet.Fleet.Position;
                var activeRegion = Context.Fleet.GetActiveRegion();
                if (_CachedDestination == null 
                    || _CachedDestination == Context.Fleet.Fleet.Position 
                    || !Context.Fleet.GetActiveRegion().Contains(_CachedDestination))
                {
                    var options = activeRegion.Where(x => x != Context.Fleet.Fleet.Position).ToList();
                    if (options.Count == 0)
                    {
                        _CachedDestination = null;
                    }
                    else if (options.Count == 1)
                    {
                        _CachedDestination = options[0];
                    }
                    else
                    {
                        _CachedDestination = options[Context.World.Random.Next(0, options.Count)];
                    }
                }
                return _CachedDestination == null 
                    ? AdansoniaNodeResult<INavigable>.Incomplete()
                    : AdansoniaNodeResult<INavigable>.Complete(_CachedDestination);
            }
        }

        private class PatrolSpotNode : ISupplierNode<IAction, FleetContext>
        {
            private readonly ISupplierNode<Fleet, FleetContext> _Target;

            public PatrolSpotNode(ISupplierNode<Fleet, FleetContext> Target)
            {
                _Target = Target;
            }

            public AdansoniaNodeResult<IAction> Execute(FleetContext Context)
            {
                var currentPosition = Context.Fleet.Fleet.Position;
                var target = _Target.Execute(Context);
                if (!target.Status.Complete)
                {
                    return AdansoniaNodeResult<IAction>.NotRun();
                }
                if (target.Result.Position == currentPosition)
                {
                    return AdansoniaNodeResult<IAction>.Complete(new SpotAction(target.Result));
                }
                return AdansoniaNodeResult<IAction>.Incomplete();
            }
        }

        private class PatrolTargetNode : ISupplierNode<Fleet, FleetContext>
        {
            private Fleet _CachedTarget;

            public AdansoniaNodeResult<Fleet> Execute(FleetContext Context)
            {
                var currentPosition = Context.Fleet.Fleet.Position;
                var faction = Context.Fleet.Fleet.Faction;
                var activeRegions = Context.Fleet.GetActiveRegion();
                if (_CachedTarget == null 
                    || !activeRegions.Contains(_CachedTarget.Position) 
                    || !Context.World.BattleManager.CanEngage(Context.Fleet.Fleet, _CachedTarget))
                {
                    var options =
                        Context.World.GetFleets()
                            .Where(x => x.Position == currentPosition)
                            .Where(x => activeRegions.Contains(x.Position))
                            .Where(x => x.Faction != faction)
                            .Where(x => Context.World.BattleManager.CanEngage(Context.Fleet.Fleet, x))
                            .ToList();
                    if (options.Count == 0)
                    {
                        _CachedTarget = null;
                    }
                    else if (options.Count == 1)
                    {
                        _CachedTarget = options[0];
                    }
                    else
                    {
                        _CachedTarget = options[Context.World.Random.Next(0, options.Count)];
                    }
                }
                return _CachedTarget == null
                    ? AdansoniaNodeResult<Fleet>.Incomplete()
                    : AdansoniaNodeResult<Fleet>.Complete(_CachedTarget);
            }
        }
        public static ISupplierNode<IAction, FleetContext> Create()
        {
            var targetBuffer = new PatrolTargetNode().Buffer();
            return new SelectorNode<AdansoniaNodeResult<IAction>, FleetContext>(
                x => x.Status.Complete, AdansoniaNodeResult<IAction>.NotRun()) {
                targetBuffer.Recompute().Check(
                    (x, y) =>
                        y.Fleet.Fleet.Position == x.Position 
                        && y.World.GetIntelligenceFor(y.Fleet.Fleet.Faction).FleetIntelligence.IsSpotted(x))
                    .Transform(EngageAction.Create),
                targetBuffer.Check((x, y) => y.Fleet.Fleet.Position == x.Position).Transform(SpotAction.Create),
                new MoveNode(
                   new SelectorNode<AdansoniaNodeResult<INavigable>, FleetContext>(
                       x => x.Status.Complete, AdansoniaNodeResult<INavigable>.Incomplete())
                   {
                       targetBuffer.Transform(x => x.Position),
                       new PatrolNode(),
                   }.Adapt(),
                   new EnumSet<NavigableEdgeType>(NavigableEdgeType.SPACE, NavigableEdgeType.JUMP))
            }.Adapt();
        }
    }
}