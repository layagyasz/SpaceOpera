using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Orders.Formations
{
    public class CreateFleetOrder : IOrder
    {
        public Faction Faction { get; }

        public CreateFleetOrder(Faction faction)
        {
            Faction = faction;
        }

        public bool Execute(World world)
        {
            var fleet = new Fleet(Faction);
            fleet.SetName(Faction.NameGenerator.GenerateNameForFleet(world.Random));
            world.Formations.AddFleet(fleet);
            return true;
        }

        public ValidationFailureReason Validate(World world)
        {
            return ValidationFailureReason.None;
        }
    }
}
