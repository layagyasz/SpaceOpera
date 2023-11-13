using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Economics.Projects
{
    public class MobilizeDivisionProject : TimedProject
    {
        public override object Key => Division;
        public override string Name => $"Mobilize {Division.Name}"; 
        public EconomicSubzoneHolding Holding { get; }
        public Division Division { get; }

        public MobilizeDivisionProject(EconomicSubzoneHolding holding, Division division)
            : base(10)
        {
            Holding = holding;
            Division = division;
        }

        protected override void CancelImpl()
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
            // TODO: Reimplement division mobilization
            // Holding.Parent.RemoveDivision(Division);
            // Division.SetPosition(Holding.Region.Center);
        }
    }
}
