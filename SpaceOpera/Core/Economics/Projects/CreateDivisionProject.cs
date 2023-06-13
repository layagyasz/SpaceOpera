using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Economics.Projects
{
    public class CreateDivisionProject : TimedProject
    {
        public override object Key => Division;
        public override string Name => $"Create {Division.Name}";
        public StellarBodyHolding Holding { get; }
        public Division Division { get; }

        public CreateDivisionProject(StellarBodyHolding holding, Division division)
            : base(10)
        {
            Holding = holding;
            Division = division;
        }

        public override void Cancel()
        {
            Holding.RemoveProject(this);
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Finish()
        {
            Holding.RemoveProject(this);
            Holding.AddDivision(Division);
        }
    }
}
