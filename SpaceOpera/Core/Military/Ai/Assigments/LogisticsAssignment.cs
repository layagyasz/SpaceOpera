using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class LogisticsAssignment : IAssignment
    {
        private class ExchangeNode : ISupplierNode<IAction, FormationContext>
        {
            private readonly LogisticsAssignment _parent;

            public ExchangeNode(LogisticsAssignment parent)
            {
                _parent = parent;
            }

            public BehaviorNodeResult<IAction> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.AtomicFormation.Position;
                var anchor = _parent.GetAnchor();
                if (currentPosition == anchor)
                {
                    _parent._leg = (_parent._leg + 1) % 2;
                }
                return BehaviorNodeResult<IAction>.NotRun();
            }
        }

        public AssignmentType Type => AssignmentType.Logistics;

        public PersistentRoute Route { get; }

        private readonly ISupplierNode<IAction, FormationContext> _routine;
        private readonly INavigable _leftAnchor;
        private readonly INavigable _rightAnchor;

        private int _leg;

        public LogisticsAssignment(PersistentRoute route)
        {
            Route = route;
            _leftAnchor = GetExchangePosition(Route.LeftAnchor);
            _rightAnchor = GetExchangePosition(Route.RightAnchor);

            _routine =
                new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun())
                {
                    new ExchangeNode(this),
                    new MoveNode(
                        SourceNode<INavigable?, FormationContext>.Wrap(GetAnchor),
                        new EnumSet<NavigableEdgeType>(NavigableEdgeType.Space, NavigableEdgeType.Jump))
                }.Adapt();
        }

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>()
            {
                ((StellarBodyRegionHolding)Route.LeftAnchor).Region.Center,
                ((StellarBodyRegionHolding)Route.RightAnchor).Region.Center
            };
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }

        private INavigable GetAnchor()
        {
            return _leg == 0 ? _leftAnchor : _rightAnchor;
        }

        private static INavigable GetExchangePosition(EconomicSubzone anchor)
        {
            var region = ((StellarBodyRegionHolding)anchor).Region;
            return region.Parent!.OrbitRegions.FirstOrDefault(x => x.SubRegions.Contains(region.Center))!;
        }
    }
}
