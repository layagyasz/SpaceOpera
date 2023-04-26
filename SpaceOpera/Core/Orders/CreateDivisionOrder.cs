using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders
{
    public class CreateDivisionOrder : IOrder
    {
        public StellarBodyHolding Holding { get; }
        public DivisionTemplate Template { get; }

        public CreateDivisionOrder(StellarBodyHolding holding, DivisionTemplate template)
        {
            Holding = holding;
            Template = template;
        }

        public ValidationFailureReason Validate()
        {
            return ValidationFailureReason.None;
        }

        public bool Execute(World world)
        {
            // TODO: Set name
            var division = new Division(Holding.Owner, Template);
            world.AddProject(new CreateDivisionProject(Holding, division));
            return true;
        }
    }
}