using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public class BuildProject : BaseProject
    {
        public override object Key => Construction.Key;
        public override string Name => $"Build {Construction.Value} x {Construction.Key.Name}";
        public override Pool Progress { get; }
        public EconomicSubzoneHolding Holding { get; }
        public Count<Structure> Construction { get; }
        public MultiQuantity<IMaterial> Cost { get; }

        public BuildProject(EconomicSubzoneHolding holding, Count<Structure> construction)
        {
            Holding = holding;
            Construction = construction;
            Progress = new(construction.Key.BuildTime, /* startFull= */ false);
            Cost = construction.Value * construction.Key.Cost;
        }

        public override void Setup()
        {
            Holding.AddProject(this);
            Holding.ReserveStructureNodes(Construction);
        }

        public override void Finish()
        {
            Holding.RemoveProject(this);
            Holding.ReleaseStructureNodes(Construction);
            Holding.AddStructures(Construction);
        }

        protected override void CancelImpl()
        {
            Holding.Parent.Return(Progress.PercentFull() * Cost);
            Holding.RemoveProject(this);
            Holding.ReleaseStructureNodes(Construction);
        }

        protected override void TickImpl()
        {
            var progress = Holding.Parent.Spend(Cost, 1f / Construction.Key.BuildTime);
            Progress.Change(progress * Construction.Key.BuildTime);
            Status = progress > float.Epsilon ? ProjectStatus.InProgress : ProjectStatus.Blocked;
        }
    }
}
