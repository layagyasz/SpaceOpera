using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Economics.Projects
{
    public class MobilizeDivisionProject : TimedProject
    {
        public StellarBodyRegionHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionProject(StellarBodyRegionHolding holding, Division division)
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
            ((StellarBodyHolding)Holding.Parent).RemoveDivision(Division);
            Division.SetPosition(Holding.Region.Center);
        }
    }
}
