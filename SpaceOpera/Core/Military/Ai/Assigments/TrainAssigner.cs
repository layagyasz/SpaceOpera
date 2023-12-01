using SpaceOpera.Core.Orders.Formations.Assignments;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Ai.Assigments
{
    public class TrainAssigner : PeriodicAssigner
    {
        public override AssignmentType Type => AssignmentType.Train;

        public TrainAssigner()
            : base(10) { }

        public override void SetActiveRegion(IEnumerable<INavigable> region) { }

        public override ICollection<INavigable> GetActiveRegion()
        {
            return new List<INavigable>();
        }

        protected override void TickImpl(ICollection<AtomicFormationDriver> drivers, SpaceOperaContext context)
        {
            foreach (var driver in drivers)
            {
                context.World.Execute(new SetAssignmentOrder(driver, Type));
            }
        }
    }
}
