using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;

namespace SpaceOpera.Core.Orders
{
    class BuildOrder : IOrder
    {
        public StellarBodyRegionHolding Holding { get; }
        public MultiCount<Structure> Structures { get; }

        public BuildOrder(StellarBodyRegionHolding holding, MultiCount<Structure> structures)
        {
            Holding = holding;
            Structures = structures;
        }

        public ValidationFailureReason Validate()
        {
            foreach (var construction in Structures)
            {
                if (construction.Value < 0)
                {
                    return ValidationFailureReason.IllegalOrder;
                }
            }
            return Structures.GetTotal() <= Holding.GetAvailableStructureNodes() 
                ? ValidationFailureReason.None 
                : ValidationFailureReason.TooFewStructureNodes;
        }

        public bool Execute(World world)
        {
            foreach (var construction in Structures.GetCounts())
            {
                world.AddProject(new BuildProject(Holding, construction));
            }
            return true;
        }
    }
}