using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Economics.Projects
{
    public class CreateDivisionProject : TimedProject
    {
        public override object Key => Division;
        public override string Name => $"Create {Division.Name}";
        public EconomicZoneHolding Holding { get; }
        public Division Division { get; }

        public CreateDivisionProject(EconomicZoneHolding holding, Division division)
            : base(10)
        {
            Holding = holding; 
            Division = division;
        }

        protected override void CancelImpl()
        {
            // Holding.RemoveProject(this);
        }

        public override void Setup()
        {
            // Holding.AddProject(this);
        }

        public override void Finish()
        {
            // Holding.RemoveProject(this);
            // TODO: Reimplement division creation
            // Holding.AddDivision(Division);
        }
    }
}
