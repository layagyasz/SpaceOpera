using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public class BuildProject : BaseResourcedProject
    {
        public override object Key => Construction.Key;
        public override string Name => $"Build {Construction.Value} x {Construction.Key.Name}";
        public Count<Structure> Construction { get; }

        public BuildProject(EconomicSubzoneHolding holding, Count<Structure> construction)
            : base(holding, construction.Key.BuildTime, construction.Value * construction.Key.Cost)
        {
            Construction = construction;
        }

        public override void Setup()
        {
            base.Setup();
            Holding.ReserveStructureNodes(Construction);
        }

        public override void Finish(World world)
        {
            base.Finish(world);
            Holding.ReleaseStructureNodes(Construction);
            Holding.AddStructures(Construction);
        }

        protected override void CancelImpl()
        {
            base.CancelImpl();
            Holding.Parent.Return(Progress.PercentFull() * Cost);
        }

    }
}
