using Cardamom.Graphing;
using SpaceOpera.Core.Orders.Formations.Assignments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class DefendAssigner : PeriodicAssigner
    {
        public override AssignmentType Type => AssignmentType.Defend;

        private readonly List<StellarBodySubRegion> _region = new();

        public DefendAssigner() 
            : base(10) { }

        public override ICollection<INavigable> GetActiveRegion()
        {
            return _region.Cast<INavigable>().ToList();
        }

        public override void SetActiveRegion(IEnumerable<INavigable> region)
        {
            _region.Clear();
            _region.AddRange(region.Cast<StellarBodySubRegion>());
            _region.Sort((x, y) => x.Center.Z.CompareTo(y.Center.Z));
            Reset();
        }

        protected override void TickImpl(ICollection<AtomicFormationDriver> drivers, SpaceOperaContext context) 
        {
            if (_region.Count == 0)
            {
                return;
            }
            float space = 1f * _region.Count / (drivers.Count + 1);
            var positions = new List<StellarBodySubRegion>();
            for (int i = 0; i < drivers.Count; ++i)
            {
                positions.Add(_region[(int)((i + 1) * space)]);
            }
            var assignments = 
                MinimalCostAssignment.ComputeGreedy(
                    drivers,
                    positions, 
                    (x, y) => context.World.NavigationMap.GetHeuristicDistance(x.AtomicFormation.Position!, y));
            foreach (var assignment in assignments)
            {
                context.World.Execute(new MoveOrder(assignment.Item1, assignment.Item2));
            }
        }
    }
}
