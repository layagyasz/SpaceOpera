using Cardamom.Collections;
using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Military.Ai.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class RetreatAssignment : IAssignment
    {
        private class RetreatFromNode : ISupplierNode<bool, FormationContext>
        {
            private INavigable? _cached;

            public BehaviorNodeResult<bool> Execute(FormationContext context)
            {
                _cached ??= context.Driver.AtomicFormation.Position;
                return BehaviorNodeResult<bool>.Complete(_cached != context.Driver.AtomicFormation.Position);
            }
        }

        private class RetreatRegionNode : ISupplierNode<INavigable?, FormationContext>
        {
            private INavigable? _cachedRegion;

            public BehaviorNodeResult<INavigable?> Execute(FormationContext context)
            {
                if (_cachedRegion != null && context.World.Formations.GetFormationsIn(_cachedRegion).Any())
                {
                    _cachedRegion = null;
                }
                if (_cachedRegion == null)
                {
                    _cachedRegion = SelectRegion(context.Driver.AtomicFormation, context.World);
                }
                return _cachedRegion == null
                    ? BehaviorNodeResult<INavigable?>.Incomplete()
                    : BehaviorNodeResult<INavigable?>.Complete(_cachedRegion);
            }

            private static INavigable? SelectRegion(IAtomicFormation formation, World world)
            {
                var allowedEdges = new EnumSet<NavigableEdgeType>(NavigableEdgeType.Ground);
                var costs =
                    new Dictionary<INavigable, float>(
                        world.NavigationMap
                            .GetNeighbors(formation.Position!, allowedEdges)
                            .Where(x => x != formation.Position)
                            .Select(x => new KeyValuePair<INavigable, float>(x, 0f)));
                foreach (var location in costs.Keys.SelectMany(x => world.NavigationMap.GetNeighbors(x, allowedEdges)))
                {
                    foreach (var f 
                        in world.Formations.GetFormationsIn(location)
                            .Where(x => world.DiplomaticRelations.CanAttack(formation.Faction, x.Formation.Faction)))
                    {
                        var power = f.AtomicFormation.GetMilitaryPower();
                        if (costs.ContainsKey(f.AtomicFormation.Position!))
                        {
                            costs[f.AtomicFormation.Position!] = float.MaxValue;
                        }
                        foreach (var neighbor in 
                            world.NavigationMap.GetNeighbors(f.AtomicFormation.Position!, allowedEdges))
                        {
                            if (costs.ContainsKey(neighbor))
                            {
                                costs[neighbor] += power;
                            }
                        }
                    }
                }
                return costs.Where(x => x.Value < float.MaxValue).ArgMin(x => x.Value).Key;
            }
        }

        public bool IsHighPriority => true;
        public AssignmentType Type => AssignmentType.Retreat;

        private readonly ISupplierNode<IAction, FormationContext> _routine;

        public RetreatAssignment()
        {
            _routine =
                new SelectorNode<BehaviorNodeResult<IAction>, FormationContext>(
                    x => x.Status.Complete, BehaviorNodeResult<IAction>.NotRun())
                {
                    new RetreatFromNode()
                        .Check(x => x).Transform<bool, IAction, FormationContext>(x => new IdleAction(unassign: true)),
                    new MoveNode(new RetreatRegionNode(), new(NavigableEdgeType.Ground), autoAttack: false),
                    // Implement surrendering
                    SourceNode<IAction, FormationContext>.Wrap(new IdleAction(/* unassign= */ true))
                }.Adapt();
        }

        public void SetActiveRegion(IEnumerable<INavigable> region) { }

        public ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>();
        }

        public BehaviorNodeResult<IAction> Execute(FormationContext context)
        {
            return _routine.Execute(context);
        }
    }
}
