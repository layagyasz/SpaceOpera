using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Economics.Projects
{
    public class CreateDivisionProject : TimedProject
    {
        public StellarBodyHolding Holding { get; }
        public Division Division { get; }

        public CreateDivisionProject(StellarBodyHolding holding, Division division)
            : base(10)
        {
            Holding = holding;
            Division = division;
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
