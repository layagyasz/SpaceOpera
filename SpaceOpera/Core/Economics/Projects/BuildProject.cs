using Cardamom.Trackers;

namespace SpaceOpera.Core.Economics.Projects
{
    public class BuildProject : IProject
    {
        public StellarBodyRegionHolding Holding { get; }
        public Count<Structure> Construction { get; }
        public Pool Progress { get; }
        public MultiQuantity<IMaterial> Cost { get; }

        public BuildProject(StellarBodyRegionHolding holding, Count<Structure> construction)
        {
            Holding = holding;
            Construction = construction;
            Progress = new(construction.Key.BuildTime);
            Cost = construction.Value * construction.Key.Cost;
        }

        public void Setup()
        {
            Holding.AddProject(this);
            Holding.ReserveStructureNodes(Construction);
        }

        public void Tick()
        {
            var progress = Holding.Parent.Spend(Cost, 1f / Construction.Key.BuildTime);
            Progress.Change(progress);
        }

        public void Finish()
        {
            Holding.RemoveProject(this);
            Holding.ReleaseStructureNodes(Construction);
            Holding.AddStructures(Construction);
        }

        public bool IsDone()
        {
            return Progress.IsFull();
        }
    }
}
