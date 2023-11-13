using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders
{
    public class CreateDivisionOrder : IOrder
    {
        public EconomicZoneHolding Holding { get; }
        public DivisionTemplate Template { get; }

        public CreateDivisionOrder(EconomicZoneHolding holding, DivisionTemplate template)
        {
            Holding = holding;
            Template = template;
        }

        public ValidationFailureReason Validate(World world)
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            // TODO: Set name
            var division = new Division(Holding.Parent.Owner, Template);
            world.Projects.Add(new CreateDivisionProject(Holding, division));
            return true;
        }
    }
}