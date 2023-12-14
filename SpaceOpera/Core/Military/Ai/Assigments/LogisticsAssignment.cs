using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using Cardamom.Trackers;
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

            private bool _finishedUnload;

            public ExchangeNode(LogisticsAssignment parent)
            {
                _parent = parent;
            }

            public BehaviorNodeResult<IAction> Execute(FormationContext context)
            {
                var currentPosition = context.Driver.AtomicFormation.Position;
                var anchor = _parent.GetExchangePosition();
                if (currentPosition == anchor)
                {
                    if (!_finishedUnload)
                    {
                        if (context.Driver.AtomicFormation.Inventory.IsEmpty() 
                            || (context.Driver.GetCurrentActionStatus() == ActionStatus.Blocked 
                                && context.Driver.GetCurrentAction().Type == ActionType.Unload))
                        {
                            _finishedUnload = true;
                        }
                        else
                        {
                            return BehaviorNodeResult<IAction>.Complete(new UnloadAction(_parent.GetAnchor()));
                        }
                    }
                    if (_finishedUnload)
                    {
                        if (context.Driver.AtomicFormation.Inventory.Contains(_parent.GetMaterials())
                            || (context.Driver.GetCurrentActionStatus() == ActionStatus.Blocked
                                && context.Driver.GetCurrentAction().Type == ActionType.Load))
                        {
                            _finishedUnload = false;
                            _parent._leg = (_parent._leg + 1) % 2;
                        }
                        else
                        {
                            return BehaviorNodeResult<IAction>.Complete(
                                new LoadAction(_parent.GetAnchor(), _parent.GetMaterials()));
                        }
                    }
                }
                return BehaviorNodeResult<IAction>.NotRun();
            }
        }

        public bool IsHighPriority => false;
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
                        SourceNode<INavigable?, FormationContext>.Wrap(GetExchangePosition),
                        new EnumSet<NavigableEdgeType>(NavigableEdgeType.Space, NavigableEdgeType.Jump),
                        autoAttack: false)
                }.Adapt();
        }

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>()
            {
                Route.LeftAnchor.Region.Center,
                Route.RightAnchor.Region.Center
            };
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }

        private EconomicZoneHolding GetAnchor()
        {
            return _leg == 0 ? Route.LeftAnchor.Parent : Route.RightAnchor.Parent;
        }

        private INavigable GetExchangePosition()
        {
            return _leg == 0 ? _leftAnchor : _rightAnchor;
        }

        private MultiQuantity<IMaterial> GetMaterials()
        {
            return _leg == 0 ? Route.LeftMaterials : Route.RightMaterials;
        }

        private static INavigable GetExchangePosition(EconomicSubzoneHolding anchor)
        {
            var region = anchor.Region;
            return region.Parent!.OrbitRegions.FirstOrDefault(x => x.SubRegions.Contains(region.Center))!;
        }
    }
}
