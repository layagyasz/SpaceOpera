using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;

namespace SpaceOpera.Core.Orders.Formations
{
    public class CreateDivisionOrder : IOrder
    {
        public EconomicSubzoneHolding Holding { get; }
        public DivisionTemplate Template { get; }

        public CreateDivisionOrder(EconomicSubzoneHolding holding, DivisionTemplate template)
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
            world.Projects.Add(
                new CreateDivisionProject(
                    Holding,
                    Holding.Owner,
                    Holding.Owner.NameGenerator.GenerateNameFor(Template, world.Random),
                    Template));
            return true;
        }
    }
}